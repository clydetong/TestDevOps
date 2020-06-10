using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using 期末專題_圖片平台.Models;

namespace 期末專題_圖片平台.Controllers
{
    public class WorkSpaceController : Controller
    {
        CWorkSpace_Factory factory = new CWorkSpace_Factory();

        // GET: WorkSpace
        public ActionResult Index()
        {
            CWorkSpace_Factory factory = new CWorkSpace_Factory();
            return View(factory.wGet所有作品());
        }
        
        public ActionResult w新增作品()
        {
            return View();
        }
        //[HttpPost]
        //public ActionResult w新增作品(HttpPostedFileBase 圖片檔案, CWorkSpace w)
        //{
        //    string fileName = "";
        //    string filePath = "";
        //    if (圖片檔案 != null)
        //    {
        //        if (圖片檔案.ContentLength > 0)
        //        {
        //            fileName = Path.GetFileName(圖片檔案.FileName);
        //            var fileExtension = Path.GetExtension(圖片檔案.FileName);
        //            var fileWithoutExtension = Path.GetFileNameWithoutExtension(圖片檔案.FileName);
        //            filePath = Path.Combine(Server.MapPath("~/Upload_image"),圖片檔案.FileName);
        //            圖片檔案.SaveAs(filePath);
                    
        //            w.圖片名稱 = fileName;
        //            w.圖片路徑 = "~/Upload_image/" + 圖片檔案.FileName;
        //            w.上傳日期 = (new CWorkSpace_Factory()).w上傳時間();
        //            if (string.IsNullOrEmpty(w.姓名))
        //                w.姓名 = "未填寫";
        //            if (string.IsNullOrEmpty(w.作品名稱))
        //                w.作品名稱 = "未填寫";
        //            if (string.IsNullOrEmpty(w.作品描述))
        //                w.作品描述 = "未填寫";
        //            if (string.IsNullOrEmpty(w.分類))
        //                w.分類 = "未填寫";
        //            if (string.IsNullOrEmpty(w.勾選項目分類))
        //                w.勾選項目分類 = "未填寫";
        //            (new CWorkSpace_Factory()).w新增作品(w);
        //        }
        //    }
        //    return RedirectToAction("Index");
        //}
        public ActionResult w瀏覽作品()
        {
            return View(factory.wGet所有作品());
        }
        public ActionResult w我的作品(string name)
        {
            return View(factory.wGetBy姓名(name));
        }
        public ActionResult w搜尋我的作品(string keyword)
        {
            return View(factory.wGetBy關鍵字(keyword));
        }

        public ActionResult w搜尋作品(string keyword)
        {
            return View(factory.wGetBy關鍵字(keyword));
        }
        [HttpGet]
        public ActionResult w修改作品資料(int id)
        {
            CWorkSpace myWork = (new CWorkSpace_Factory()).wGetBy編號(id);
            if(myWork == null)
            {
                return RedirectToAction("Index");
            }
            return View(myWork);
        }

        [HttpPost]
        public ActionResult w新增作品(HttpPostedFileBase 圖片檔案, CWorkSpace w)
        {
            string fileName = "";
            string filePath = "";
            if (圖片檔案 != null)
            {
                if (圖片檔案.ContentLength > 0)
                {
                    fileName = Path.GetFileName(圖片檔案.FileName);
                    var fileExtension = Path.GetExtension(圖片檔案.FileName);
                    var fileWithoutExtension = Path.GetFileNameWithoutExtension(圖片檔案.FileName);
                    filePath = Path.Combine(Server.MapPath("~/Upload_image"), 圖片檔案.FileName);
                    圖片檔案.SaveAs(filePath);


                    w.圖片名稱 = fileName;

                    //回覆訊息

                    byte[] imgData;

                    using (var reader = new BinaryReader(圖片檔案.InputStream))
                    {
                        imgData = reader.ReadBytes(圖片檔案.ContentLength);
                    }

                    //var bytes = ImageToByte2(圖片檔案);
                    var ImgurRet = UploadImage2Imgur(imgData);

                    /////
                    /////

                    w.圖片路徑 = (new Uri(""+ ImgurRet.data.link).ToString());

                    //w.圖片路徑 = "";
                    w.上傳日期 = (new CWorkSpace_Factory()).w上傳時間();
                    if (string.IsNullOrEmpty(w.姓名))
                        w.姓名 = "未填寫";
                    if (string.IsNullOrEmpty(w.作品名稱))
                        w.作品名稱 = "未填寫";
                    if (string.IsNullOrEmpty(w.作品描述))
                        w.作品描述 = "未填寫";
                    if (string.IsNullOrEmpty(w.分類))
                        w.分類 = "未填寫";
                    if (string.IsNullOrEmpty(w.勾選項目分類))
                        w.勾選項目分類 = "未填寫";
                    (new CWorkSpace_Factory()).w新增作品(w);
                }
            }
            return RedirectToAction("w新增作品");
        }

        static dynamic UploadImage2Imgur(byte[] image)
        {
            HttpClient client = new HttpClient();
            string uriBase = "https://api.imgur.com/3/upload";

            // Request headers.
            client.DefaultRequestHeaders.Add(
                "Authorization", $"Client-ID 317c12e82db1240");

            string uri = uriBase;

            HttpResponseMessage response;

            // Add the byte array as an octet stream to the request body.
            using (ByteArrayContent content = new ByteArrayContent(image))
            {
                // This example uses the "application/octet-stream" content type.
                // The other content types you can use are "application/json"
                // and "multipart/form-data".
                content.Headers.ContentType =
                    new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

                // Asynchronously call the REST API method.
                response = client.PostAsync(uri, content).Result;
            }

            // Asynchronously get the JSON response.
            string JSON = response.Content.ReadAsStringAsync().Result;

            return Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(JSON);
        }

        static byte[] ImageToByte2(Image img)
        {
            using (var stream = new System.IO.MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }



        //[HttpPost]
        //public ActionResult w修改作品資料(HttpPostedFileBase 圖片檔案, CWorkSpace w ,int id)
        //{
        //    CWorkSpace_Factory factory = new CWorkSpace_Factory();
        //    if (圖片檔案 != null)
        //    {
        //        if (圖片檔案.ContentLength > 0)
        //        {
        //            var fileName = Path.GetFileName(圖片檔案.FileName);
        //            var fileExtension = Path.GetExtension(圖片檔案.FileName);
        //            var fileWithoutExtension = Path.GetFileNameWithoutExtension(圖片檔案.FileName);
        //            var filePath = Path.Combine(Server.MapPath("~/Upload_image"), 圖片檔案.FileName);
        //            圖片檔案.SaveAs(filePath);

        //            w.圖片名稱 = fileName;
        //            w.圖片路徑 = "~/Upload_image/"+圖片檔案.FileName;
        //            w.上傳日期 = (new CWorkSpace_Factory()).w上傳時間();

        //            factory.w修改作品資料(w, id);
        //        }
        //    }

        //    return RedirectToAction("Index");
        //}


        public ActionResult w刪除作品(int id)
        {
            CWorkSpace_Factory factory = new CWorkSpace_Factory();
            factory.w刪除作品By編號(id);
            return RedirectToAction("Index");
        }
    }
}