using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smartway_Test.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smartway_Test.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Files()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadFiles(IEnumerable<IFormFile> FormFile) //загрузка файлов
        {
            if (FormFile.Count() != 0) 
            {
                List<File> DbFiles = new List<File>(FormFile.Count()); //список файлов
               
                foreach (var file in FormFile) //преобразуем файлы в байты и добавляем в список
                {                    
                    using var binaryReader = new System.IO.BinaryReader(file.OpenReadStream());
                    byte[] fcontent = binaryReader.ReadBytes((int)file.Length);

                    DbFiles.Add(new File
                    {
                        Name = file.FileName.Split('.')[0],
                        Size = file.Length,
                        Format = file.FileName.Split('.')[1],
                        Content = fcontent,
                        Links = new List<Link>()
                    });
                }

                //сохраняем список файлов в БД
                using AppDbContext context = new AppDbContext();
                await context.Files.AddRangeAsync(DbFiles);
                await context.SaveChangesAsync();
            }
            return View("Files");
        }
    
        [HttpPost]
        public async Task<IActionResult> DownloadFiles(int[] filesId) //скачивание выбранных файлов 
        {
            if (filesId.Length == 0) return View("Files");
            //получаем все элементы у которых Id в списке
            using AppDbContext context = new AppDbContext();
            var files = await context.Files.Where(x => filesId.Contains(x.Id)).ToListAsync();

            if (files.Count == 1) //если файл 1 его возвращаем
            {
                return new FileContentResult(files[0].Content, $"application/{files[0].Format}") { FileDownloadName = $"{files[0].Name}.{files[0].Format}" };
            } 
            else //если неск-ко - кидаем в архив
            {
                using System.IO.MemoryStream ms = new System.IO.MemoryStream();
                using (var archive = new System.IO.Compression.ZipArchive(ms, System.IO.Compression.ZipArchiveMode.Create, true))
                {
                    foreach (var file in files)
                    {
                        var entry = archive.CreateEntry(file.Name + "." + file.Format, System.IO.Compression.CompressionLevel.Fastest);
                        using var zipStream = entry.Open();
                        await zipStream.WriteAsync(file.Content, 0, (int)file.Size);
                    }
                }
                return File(ms.ToArray(), "application/zip", "Files.zip");
            }
        }

        [HttpPost]
        public async Task<string> GenerateLink(int filesId) //генерация уникальной ссылки
        {
            //генерация уникального 24-тизнач. идентификатора вида: UcBKmq2XE5aА
            StringBuilder builder = new StringBuilder();
            Enumerable
               .Range(65, 26)
                .Select(e => ((char)e).ToString())
                .Concat(Enumerable.Range(97, 26).Select(e => ((char)e).ToString()))
                .Concat(Enumerable.Range(0, 10).Select(e => e.ToString()))
                .OrderBy(e => Guid.NewGuid())
                .Take(24)
                .ToList().ForEach(e => builder.Append(e));
            string hash = builder.ToString();
            //сохранение уникального хеша
            using AppDbContext context = new AppDbContext();
            var file = await context.Files.Include(x => x.Links).FirstOrDefaultAsync(x => x.Id == filesId);
            file.Links.Add(new Link { Hash = hash });
            await context.SaveChangesAsync();
            //возврат ссылки для скачивания файла по уник. хешу
            return $"https://{HttpContext.Request.Host.Value}/Home/DownloadLink?hash={hash}";
        }

        public async Task<IActionResult> DownloadLink(string hash) //скачивание файла по уник.ссылке
        {
            //поиск файла с указыннм хешем ссылки
            using AppDbContext context = new AppDbContext();
            var link = await context.Links.Include(x => x.File).FirstOrDefaultAsync(x => x.Hash == hash);
            if (link != null)
            {
                //сохраняем файл, удаляем ссылку и возвращаем файл для скачивания
                var file = link.File;
                context.Links.Remove(link);
                await context.SaveChangesAsync();
                return new FileContentResult(file.Content, $"application/{file.Format}") { FileDownloadName = $"{file.Name}.{file.Format}" };
            }
            return Content("Ссылка недействительна");
        }


        //ссылки генерировать и использовать можно было через WebAPI
    }
}
