using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Trilhas.Services;

namespace Trilhas.Controllers
{
    [Route("fileman")]
    public class FilemanController : Controller
    {
        private readonly MinioService _service;
        private Dictionary<string, string> _settings = null;
        private Dictionary<string, string> _lang = null;
        private HttpResponse _r = null;
        private HttpContext _context = null;
        private string confFile = @"fileman\conf.json";
        private string rootUrl = "wwwroot";

        public FilemanController(MinioService service)
        {
            _service = service;
        }

        [HttpPost, HttpGet]
        [Route("asp_net/main.ashx")]
        public async Task ProcessRequestPost()
        {
            _context = HttpContext;
            _r = Response;
            string action = "DIRLIST";
            try
            {
                if (_context.Request.Query["a"].ToString() != null)
                    action = _context.Request.Query["a"].ToString();

                VerifyAction(action);
                switch (action.ToUpper())
                {
                    case "DIRLIST":
                        await ListDirTree(_context.Request.Query["type"]);
                        break;
                    case "FILESLIST":
                        await ListFiles(_context.Request.Form["d"], _context.Request.Form["type"]);
                        break;
                    case "COPYDIR":
                        await CopyDir(_context.Request.Form["d"], _context.Request.Form["n"]);
                        break;
                    case "COPYFILE":
                        await CopyFile(_context.Request.Form["f"], _context.Request.Form["n"]);
                        break;
                    case "CREATEDIR":
                        await CreateDir(_context.Request.Form["d"], _context.Request.Form["n"]);
                        break;
                    case "DELETEDIR":
                        await DeleteDir(_context.Request.Form["d"]);
                        break;
                    case "DELETEFILE":
                        await DeleteFile(_context.Request.Form["f"]);
                        break;
                    //case "DOWNLOAD":
                    //    return DownloadFile(_context.Request.Query["f"]);
                    //case "DOWNLOADDIR":
                    //    return DownloadDir(_context.Request.Query["d"]);
                    case "MOVEDIR":
                        await MoveDir(_context.Request.Form["d"], _context.Request.Form["n"]);
                        break;
                    case "MOVEFILE":
                        await MoveFile(_context.Request.Form["f"], _context.Request.Form["n"]);
                        break;
                    case "RENAMEDIR":
                        await RenameDir(_context.Request.Form["d"], _context.Request.Form["n"]);
                        break;
                    case "RENAMEFILE":
                        await RenameFile(_context.Request.Form["f"], _context.Request.Form["n"]);
                        break;
                    case "GENERATETHUMB":
                        int w = 140, h = 0;
                        int.TryParse(_context.Request.Query["width"].ToString().Replace("px", ""), out w);
                        int.TryParse(_context.Request.Query["height"].ToString().Replace("px", ""), out h);
                        ShowThumbnail(_context.Request.Query["f"], w, h);
                        break;
                    case "UPLOAD":
                        await Upload(_context.Request.Form["d"]);
                        break;
                    default:
                        await _r.WriteAsync(GetErrorRes("This action is not implemented."));
                        break;
                }

            }
            catch (Exception ex)
            {
                if (action == "UPLOAD" && !IsAjaxUpload())
                {
                    await _r.WriteAsync("<script>");
                    await _r.WriteAsync("parent.fileUploaded(" + GetErrorRes(LangRes("E_UploadNoFiles")) + ");");
                    await _r.WriteAsync("</script>");
                }
                else
                {
                    await _r.WriteAsync(GetErrorRes(ex.Message));
                }
            }
        }

        private string FixPath(string path)
        {
            //if (!path.StartsWith("~"))
            //{
            //    if (!path.StartsWith("/"))
            //        path = "/" + path;
            //    path = "~" + path;
            //}
            if (path.StartsWith("/"))
            {
                path = path.Substring(1);
            }
            path = path.Replace("/", "\\");
            return path; // Path.Combine(rootUrl, path);
        }
        private string GetLangFile()
        {
            string filename = @"fileman\lang\" + GetSetting("LANG") + ".json";
            if (!System.IO.File.Exists(Path.Combine(rootUrl, filename)))
            {
                filename = @"fileman\lang\en.json";
            }
            return filename;
        }
        protected string LangRes(string name)
        {
            string ret = name;
            if (_lang == null)
                _lang = ParseJSON(GetLangFile());
            if (_lang.ContainsKey(name))
                ret = _lang[name];

            return ret;
        }
        protected string GetFileType(string ext)
        {
            string ret = "file";
            ext = ext.ToLower();
            if (ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".gif")
                ret = "image";
            else if (ext == ".swf" || ext == ".flv")
                ret = "flash";
            return ret;
        }
        protected bool CanHandleFile(string filename)
        {
            bool ret = false;
            FileInfo file = new FileInfo(filename);
            string ext = file.Extension.Replace(".", "").ToLower();
            string setting = GetSetting("FORBIDDEN_UPLOADS").Trim().ToLower();
            if (setting != "")
            {
                ArrayList tmp = new ArrayList();
                tmp.AddRange(Regex.Split(setting, "\\s+"));
                if (!tmp.Contains(ext))
                    ret = true;
            }
            setting = GetSetting("ALLOWED_UPLOADS").Trim().ToLower();
            if (setting != "")
            {
                ArrayList tmp = new ArrayList();
                tmp.AddRange(Regex.Split(setting, "\\s+"));
                if (!tmp.Contains(ext))
                    ret = false;
            }

            return ret;
        }
        protected Dictionary<string, string> ParseJSON(string file)
        {
            Dictionary<string, string> ret = new Dictionary<string, string>();
            string json = "";
            try
            {
                json = System.IO.File.ReadAllText(Path.Combine(rootUrl, file), System.Text.Encoding.UTF8);
            }
            catch { }

            json = json.Trim();
            if (json != "")
            {
                if (json.StartsWith("{"))
                    json = json.Substring(1, json.Length - 2);
                json = json.Trim();
                json = json.Substring(1, json.Length - 2);
                string[] lines = Regex.Split(json, "\"\\s*,\\s*\"");
                foreach (string line in lines)
                {
                    string[] tmp = Regex.Split(line, "\"\\s*:\\s*\"");
                    try
                    {
                        if (tmp[0] != "" && !ret.ContainsKey(tmp[0]))
                        {
                            ret.Add(tmp[0], tmp[1]);
                        }
                    }
                    catch { }
                }
            }
            return ret;
        }
        protected string GetFilesRoot()
        {
            string ret = GetSetting("FILES_ROOT");
            //if (GetSetting("SESSION_PATH_KEY") != "" && _context.Session[GetSetting("SESSION_PATH_KEY")] != null)
            //    ret = (string)_context.Session[GetSetting("SESSION_PATH_KEY")];

            if (ret == "")
                ret = Path.Combine(rootUrl, @"fileman\Uploads");
            else
                ret = FixPath(ret);
            return ret;
        }
        protected void LoadConf()
        {
            if (_settings == null)
                _settings = ParseJSON(confFile);
        }
        protected string GetSetting(string name)
        {
            string ret = "";
            LoadConf();
            if (_settings.ContainsKey(name))
                ret = _settings[name];

            return ret;
        }
        protected void CheckPath(string path)
        {
            if (FixPath(path).IndexOf(GetFilesRoot()) != 0)
            {
                throw new Exception("Access to " + path + " is denied");
            }
        }
        protected void VerifyAction(string action)
        {
            string setting = GetSetting(action);
            if (setting.IndexOf("?") > -1)
                setting = setting.Substring(0, setting.IndexOf("?"));
            if (!setting.StartsWith("/"))
                setting = "/" + setting;
            setting = ".." + setting;

            //if (_context.Server.MapPath(setting) != _context.Server.MapPath(_context.Request.Url.LocalPath))
            //    throw new Exception(LangRes("E_ActionDisabled"));
        }
        protected string GetResultStr(string type, string msg)
        {
            return "{\"res\":\"" + type + "\",\"msg\":\"" + msg.Replace("\"", "\\\"") + "\"}";
        }
        protected string GetSuccessRes(string msg)
        {
            return GetResultStr("ok", msg);
        }
        protected string GetSuccessRes()
        {
            return GetSuccessRes("");
        }
        protected string GetErrorRes(string msg)
        {
            return GetResultStr("error", msg);
        }
        private void _copyDir(string path, string dest)
        {
            if (!Directory.Exists(dest))
                Directory.CreateDirectory(dest);
            foreach (string f in Directory.GetFiles(path))
            {
                FileInfo file = new FileInfo(f);
                if (!System.IO.File.Exists(Path.Combine(dest, file.Name)))
                {
                    System.IO.File.Copy(f, Path.Combine(dest, file.Name));
                }
            }
            foreach (string d in Directory.GetDirectories(path))
            {
                DirectoryInfo dir = new DirectoryInfo(d);
                _copyDir(d, Path.Combine(dest, dir.Name));
            }
        }
        protected async Task CopyDir(string path, string newPath)
        {
            CheckPath(path);
            CheckPath(newPath);
            DirectoryInfo dir = new DirectoryInfo(FixPath(path));
            DirectoryInfo newDir = new DirectoryInfo(FixPath(newPath + "/" + dir.Name));

            if (!dir.Exists)
            {
                throw new Exception(LangRes("E_CopyDirInvalidPath"));
            }
            else if (newDir.Exists)
            {
                throw new Exception(LangRes("E_DirAlreadyExists"));
            }
            else
            {
                _copyDir(dir.FullName, newDir.FullName);
            }
            await _r.WriteAsync(GetSuccessRes());
        }
        protected string MakeUniqueFilename(string dir, string filename)
        {
            string ret = filename;
            int i = 0;
            while (System.IO.File.Exists(Path.Combine(dir, ret)))
            {
                i++;
                ret = Path.GetFileNameWithoutExtension(filename) + " - Copy " + i.ToString() + Path.GetExtension(filename);
            }
            return ret;
        }
        protected async Task CopyFile(string path, string newPath)
        {
            CheckPath(path);
            FileInfo file = new FileInfo(FixPath(path));
            newPath = FixPath(newPath);
            if (!file.Exists)
                throw new Exception(LangRes("E_CopyFileInvalisPath"));
            else
            {
                string newName = MakeUniqueFilename(newPath, file.Name);
                try
                {
                    System.IO.File.Copy(file.FullName, Path.Combine(newPath, newName));
                    await _r.WriteAsync(GetSuccessRes());
                }
                catch
                {
                    throw new Exception(LangRes("E_CopyFile"));
                }
            }
        }
        protected async Task CreateDir(string path, string name)
        {
            CheckPath(path);
            path = FixPath(path);
            if (!Directory.Exists(path))
                throw new Exception(LangRes("E_CreateDirInvalidPath"));
            else
            {
                try
                {
                    path = Path.Combine(path, name);
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    await _r.WriteAsync(GetSuccessRes());
                }
                catch
                {
                    throw new Exception(LangRes("E_CreateDirFailed"));
                }
            }
        }
        protected async Task DeleteDir(string path)
        {
            CheckPath(path);
            path = FixPath(path);
            if (!Directory.Exists(path))
                throw new Exception(LangRes("E_DeleteDirInvalidPath"));
            else if (path == GetFilesRoot())
                throw new Exception(LangRes("E_CannotDeleteRoot"));
            else if (Directory.GetDirectories(path).Length > 0 || Directory.GetFiles(path).Length > 0)
                throw new Exception(LangRes("E_DeleteNonEmpty"));
            else
            {
                try
                {
                    Directory.Delete(path);
                    await _r.WriteAsync(GetSuccessRes());
                }
                catch
                {
                    throw new Exception(LangRes("E_CannotDeleteDir"));
                }
            }
        }
        protected async Task DeleteFile(string path)
        {
            CheckPath(path);
            path = FixPath(path);
            if (!System.IO.File.Exists(path))
            {
                throw new Exception(LangRes("E_DeleteFileInvalidPath"));
            }
            else
            {
                try
                {
                    System.IO.File.Delete(path);
                    await _r.WriteAsync(GetSuccessRes());
                }
                catch (Exception ex)
                {
                    throw new Exception(LangRes("E_DeletеFile") + " - " + ex.Message);
                }
            }
        }

        private List<string> GetFiles(string path, string type)
        {
            List<string> ret = new List<string>();
            if (type == "#")
                type = "";
            string[] files = Directory.GetFiles(path);
            foreach (string f in files)
            {
                if ((GetFileType(new FileInfo(f).Extension) == type) || (type == ""))
                    ret.Add(f);
            }
            return ret;
        }

        private ArrayList ListDirs(string path)
        {
            string[] dirs = Directory.GetDirectories(path);
            ArrayList ret = new ArrayList();
            foreach (string dir in dirs)
            {
                ret.Add(dir);
                ret.AddRange(ListDirs(dir));
            }
            return ret;
        }
        protected async Task ListDirTree(string type)
        {
            DirectoryInfo d = new DirectoryInfo(GetFilesRoot());
            if (!d.Exists)
                throw new Exception("Invalid files root directory. Check your configuration.");

            ArrayList dirs = ListDirs(d.ToString());
            dirs.Insert(0, d.ToString());

            //string localPath = _context.Server.MapPath("~/");
            string localPath = "~/";
            await _r.WriteAsync("[");
            for (int i = 0; i < dirs.Count; i++)
            {
                string dir = (string)dirs[i];

                await _r.WriteAsync(("{\"p\":\"/" + dir.Replace(localPath, "").Replace("\\", "/") + "\",\"f\":\"" + GetFiles(dir, type).Count.ToString() + "\",\"d\":\"" + Directory.GetDirectories(dir).Length.ToString() + "\"}"));
                if (i < dirs.Count - 1)
                    await _r.WriteAsync(",");
            }
            await _r.WriteAsync("]");
        }
        protected double LinuxTimestamp(DateTime d)
        {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0).ToLocalTime();
            TimeSpan timeSpan = (d.ToLocalTime() - epoch);

            return timeSpan.TotalSeconds;
        }
        protected async Task ListFiles(string path, string type)
        {
            CheckPath(path);
            string fullPath = FixPath(path);
            List<string> files = GetFiles(fullPath, type);
            await _r.WriteAsync("[");
            for (int i = 0; i < files.Count; i++)
            {
                FileInfo f = new FileInfo(files[i]);
                int w = 0, h = 0;
                if (GetFileType(f.Extension) == "image")
                {
                    try
                    {
                        FileStream fs = new FileStream(f.FullName, FileMode.Open, FileAccess.Read);
                        Image img = Image.FromStream(fs);
                        w = img.Width;
                        h = img.Height;
                        fs.Close();
                        fs.Dispose();
                        img.Dispose();
                    }
                    catch (Exception ex) { throw ex; }
                }
                await _r.WriteAsync("{");
                await _r.WriteAsync("\"p\":\"" + path + "/" + f.Name + "\"");
                await _r.WriteAsync(",\"t\":\"" + Math.Ceiling(LinuxTimestamp(f.LastWriteTime)).ToString() + "\"");
                await _r.WriteAsync(",\"s\":\"" + f.Length.ToString() + "\"");
                await _r.WriteAsync(",\"w\":\"" + w.ToString() + "\"");
                await _r.WriteAsync(",\"h\":\"" + h.ToString() + "\"");
                await _r.WriteAsync("}");
                if (i < files.Count - 1)
                    await _r.WriteAsync(",");
            }
            await _r.WriteAsync("]");

            //var arquivos = _service.ListarArquivosCkeditor();
            //await _r.WriteAsync("[");

            //for (int i = 0; i < arquivos.Count; i++)
            //{
            //    await _r.WriteAsync("{");
            //    await _r.WriteAsync("\"p\":\"" + arquivos[i].Key + "\"");
            //    await _r.WriteAsync(",\"t\":\"" + Math.Ceiling(LinuxTimestamp(arquivos[i].LastModifiedDateTime.Value)).ToString() + "\"");
            //    await _r.WriteAsync(",\"s\":\"" + arquivos[i].Size + "\"");
            //    await _r.WriteAsync("}");

            //    if (i < arquivos.Count - 1)
            //    {
            //        await _r.WriteAsync(",");
            //    }
            //}

            //await _r.WriteAsync("]");
        }
        //public IActionResult DownloadDir(string path)
        //{
        //    path = FixPath(path);
        //    if (!Directory.Exists(path))
        //        throw new Exception(LangRes("E_CreateArchive"));
        //    string dirName = new FileInfo(path).Name;
        //    string tmpZip = Path.Combine(rootUrl, @"fileman\tmp\" + dirName + ".zip");
        //    if (System.IO.File.Exists(tmpZip))
        //        System.IO.File.Delete(tmpZip);
        //    ZipFile.CreateFromDirectory(path, tmpZip, CompressionLevel.Fastest, true);

        //    using (var streamReader = new StreamReader(tmpZip))
        //    {
        //        using (var memstream = new MemoryStream())
        //        {
        //            streamReader.BaseStream.CopyTo(memstream);
        //            var bytes = memstream.ToArray();
        //            return File(bytes, System.Web.MimeMapping.GetMimeMapping(dirName + ".zip"), dirName + ".zip");
        //        }
        //    }

        //    //_r.Clear();
        //    //_r.Headers.Add("Content-Disposition", "attachment; filename=\"" + dirName + ".zip\"");
        //    //_r.ContentType = "application/force-download";
        //    //_r.TransmitFile(tmpZip);
        //    //_r.Flush();
        //    //_r.End();
        //}
        //protected IActionResult DownloadFile(string path)
        //{
        //    CheckPath(path);
        //    FileInfo file = new FileInfo(FixPath(path));
        //    if (file.Exists)
        //    {
        //        using (var streamReader = new StreamReader(file.FullName))
        //        {
        //            using (var memstream = new MemoryStream())
        //            {
        //                streamReader.BaseStream.CopyTo(memstream);
        //                var bytes = memstream.ToArray();
        //                return File(bytes, System.Web.MimeMapping.GetMimeMapping(file.Name), file.Name);
        //            }
        //        }

        //        //_r.Clear();
        //        //_r.Headers.Add("Content-Disposition", "attachment; filename=\"" + file.Name + "\"");
        //        //_r.ContentType = "application/force-download";
        //        //_r.TransmitFile(file.FullName);
        //        //_r.Flush();
        //        //_r.End();
        //    }
        //    return null;
        //}

        protected async Task MoveDir(string path, string newPath)
        {
            CheckPath(path);
            CheckPath(newPath);
            DirectoryInfo source = new DirectoryInfo(FixPath(path));
            DirectoryInfo dest = new DirectoryInfo(FixPath(Path.Combine(newPath, source.Name)));
            if (dest.FullName.IndexOf(source.FullName) == 0)
                throw new Exception(LangRes("E_CannotMoveDirToChild"));
            else if (!source.Exists)
                throw new Exception(LangRes("E_MoveDirInvalisPath"));
            else if (dest.Exists)
                throw new Exception(LangRes("E_DirAlreadyExists"));
            else
            {
                try
                {
                    source.MoveTo(dest.FullName);
                    await _r.WriteAsync(GetSuccessRes());
                }
                catch
                {
                    throw new Exception(LangRes("E_MoveDir") + " \"" + path + "\"");
                }
            }

        }
        protected async Task MoveFile(string path, string newPath)
        {
            CheckPath(path);
            CheckPath(newPath);
            FileInfo source = new FileInfo(FixPath(path));
            FileInfo dest = new FileInfo(FixPath(newPath));

            if (!source.Exists)
                throw new Exception(LangRes("E_MoveFileInvalisPath"));
            else if (dest.Exists)
                throw new Exception(LangRes("E_MoveFileAlreadyExists"));
            else if (!CanHandleFile(dest.Name))
                throw new Exception(LangRes("E_FileExtensionForbidden"));
            else
            {
                try
                {
                    source.MoveTo(dest.FullName);
                    await _r.WriteAsync(GetSuccessRes());
                }
                catch
                {
                    throw new Exception(LangRes("E_MoveFile") + " \"" + path + "\"");
                }
            }
        }
        protected async Task RenameDir(string path, string name)
        {
            CheckPath(path);
            DirectoryInfo source = new DirectoryInfo(FixPath(path));
            DirectoryInfo dest = new DirectoryInfo(Path.Combine(source.Parent.FullName, name));
            if (source.FullName == GetFilesRoot())
                throw new Exception(LangRes("E_CannotRenameRoot"));
            else if (!source.Exists)
                throw new Exception(LangRes("E_RenameDirInvalidPath"));
            else if (dest.Exists)
                throw new Exception(LangRes("E_DirAlreadyExists"));
            else
            {
                try
                {
                    source.MoveTo(dest.FullName);
                    await _r.WriteAsync(GetSuccessRes());
                }
                catch
                {
                    throw new Exception(LangRes("E_RenameDir") + " \"" + path + "\"");
                }
            }
        }
        protected async Task RenameFile(string path, string name)
        {
            CheckPath(path);
            FileInfo source = new FileInfo(FixPath(path));
            FileInfo dest = new FileInfo(Path.Combine(source.Directory.FullName, name));
            if (!source.Exists)
                throw new Exception(LangRes("E_RenameFileInvalidPath"));
            else if (!CanHandleFile(name))
                throw new Exception(LangRes("E_FileExtensionForbidden"));
            else
            {
                try
                {
                    source.MoveTo(dest.FullName);
                    await _r.WriteAsync(GetSuccessRes());
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message + "; " + LangRes("E_RenameFile") + " \"" + path + "\"");
                }
            }
        }
        public bool ThumbnailCallback()
        {
            return false;
        }

        protected void ShowThumbnail(string path, int width, int height)
        {
            CheckPath(path);
            FileStream fs = new FileStream(FixPath(path), FileMode.Open, FileAccess.Read);
            Bitmap img = new Bitmap(Bitmap.FromStream(fs));
            fs.Close();
            fs.Dispose();
            int cropWidth = img.Width, cropHeight = img.Height;
            int cropX = 0, cropY = 0;

            double imgRatio = (double)img.Width / (double)img.Height;

            if (height == 0)
                height = Convert.ToInt32(Math.Floor((double)width / imgRatio));

            if (width > img.Width)
                width = img.Width;
            if (height > img.Height)
                height = img.Height;

            double cropRatio = (double)width / (double)height;
            cropWidth = Convert.ToInt32(Math.Floor((double)img.Height * cropRatio));
            cropHeight = Convert.ToInt32(Math.Floor((double)cropWidth / cropRatio));
            if (cropWidth > img.Width)
            {
                cropWidth = img.Width;
                cropHeight = Convert.ToInt32(Math.Floor((double)cropWidth / cropRatio));
            }
            if (cropHeight > img.Height)
            {
                cropHeight = img.Height;
                cropWidth = Convert.ToInt32(Math.Floor((double)cropHeight * cropRatio));
            }
            if (cropWidth < img.Width)
            {
                cropX = Convert.ToInt32(Math.Floor((double)(img.Width - cropWidth) / 2));
            }
            if (cropHeight < img.Height)
            {
                cropY = Convert.ToInt32(Math.Floor((double)(img.Height - cropHeight) / 2));
            }

            Rectangle area = new Rectangle(cropX, cropY, cropWidth, cropHeight);
            Bitmap cropImg = img.Clone(area, System.Drawing.Imaging.PixelFormat.DontCare);
            img.Dispose();
            Image.GetThumbnailImageAbort imgCallback = new Image.GetThumbnailImageAbort(ThumbnailCallback);

            _r.Headers.Add("Content-Type", "image/png");
            cropImg.GetThumbnailImage(width, height, imgCallback, IntPtr.Zero).Save(_r.Body, ImageFormat.Png);
            _r.Body.Close();
            cropImg.Dispose();
        }

        private ImageFormat GetImageFormat(string filename)
        {
            ImageFormat ret = ImageFormat.Jpeg;
            switch (new FileInfo(filename).Extension.ToLower())
            {
                case ".png": ret = ImageFormat.Png; break;
                case ".gif": ret = ImageFormat.Gif; break;
            }
            return ret;
        }

        protected void ImageResize(string path, string dest, int width, int height)
        {
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            Image img = Image.FromStream(fs);
            fs.Close();
            fs.Dispose();
            float ratio = (float)img.Width / (float)img.Height;
            if ((img.Width <= width && img.Height <= height) || (width == 0 && height == 0))
                return;

            int newWidth = width;
            int newHeight = Convert.ToInt16(Math.Floor((float)newWidth / ratio));
            if ((height > 0 && newHeight > height) || (width == 0))
            {
                newHeight = height;
                newWidth = Convert.ToInt16(Math.Floor((float)newHeight * ratio));
            }
            Bitmap newImg = new Bitmap(newWidth, newHeight);
            Graphics g = Graphics.FromImage((Image)newImg);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(img, 0, 0, newWidth, newHeight);
            img.Dispose();
            g.Dispose();
            if (dest != "")
            {
                newImg.Save(dest, GetImageFormat(dest));
            }
            newImg.Dispose();
        }
        protected bool IsAjaxUpload()
        {
            return (!string.IsNullOrEmpty(_context.Request.Query["method"]) && _context.Request.Query["method"].ToString() == "ajax");
        }
        protected async Task Upload(string path)
        {
            CheckPath(path);
            path = FixPath(path);
            string res = GetSuccessRes();
            bool hasErrors = false;
            try
            {
                for (int i = 0; i < HttpContext.Request.Form.Files.Count; i++)
                {
                    if (CanHandleFile(HttpContext.Request.Form.Files[i].FileName))
                    {
                        FileInfo f = new FileInfo(HttpContext.Request.Form.Files[i].FileName);
                        string filename = MakeUniqueFilename(path, f.Name);
                        string dest = Path.Combine(path, filename);
                        //HttpContext.Request.Form.Files[i].SaveAs(dest);

                        using (var item = HttpContext.Request.Form.Files[i].OpenReadStream())
                        {
                            var fileStream = new FileStream(dest, FileMode.Create, FileAccess.Write);
                            item.CopyTo(fileStream);
                            fileStream.Close();
                        }

                        if (GetFileType(new FileInfo(filename).Extension) == "image")
                        {
                            int w = 0;
                            int h = 0;
                            int.TryParse(GetSetting("MAX_IMAGE_WIDTH"), out w);
                            int.TryParse(GetSetting("MAX_IMAGE_HEIGHT"), out h);
                            ImageResize(dest, dest, w, h);
                        }
                    }
                    else
                    {
                        hasErrors = true;
                        res = GetSuccessRes(LangRes("E_UploadNotAll"));
                    }
                }
            }
            catch (Exception ex)
            {
                res = GetErrorRes(ex.Message);
            }
            if (IsAjaxUpload())
            {
                if (hasErrors)
                    res = GetErrorRes(LangRes("E_UploadNotAll"));
                await _r.WriteAsync(res);
            }
            else
            {
                await _r.WriteAsync("<script>");
                await _r.WriteAsync("parent.fileUploaded(" + res + ");");
                await _r.WriteAsync("</script>");
            }
        }
    }
}