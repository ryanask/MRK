using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AskrindoMVC.Models;
using AskrindoMVC.Areas.Admin.Models.DocMgr;
using System.IO;

namespace AskrindoMVC.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DocMgrController : Controller
    {
        AskrindoMVCEntities db = new AskrindoMVCEntities();

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        public ActionResult Index()
        {
            return View(db.HelpMenus);
        }

        public ActionResult MenuNew()
        {
            return View();
        }

        [HttpPost]
        public ActionResult MenuNew(HelpMenu hm)
        {
            if (ModelState.IsValid)
            {
                db.HelpMenus.AddObject(hm);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(hm);
        }

        public ActionResult MenuEdit(int id)
        {
            return View(db.HelpMenus.Single(p => p.MenuId == id));
        }

        [HttpPost]
        public ActionResult MenuEdit(HelpMenu hm, int id)
        {
            if (ModelState.IsValid)
            {
                db.HelpMenus.Attach(hm);
                db.ObjectStateManager.ChangeObjectState(hm, System.Data.EntityState.Modified);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(hm);
        }

        public ActionResult MenuDelete(int id)
        {
            return View(db.HelpMenus.Single(p => p.MenuId == id));
        }

        [HttpPost, ActionName("MenuDelete")]
        public ActionResult MenuDeleteConfirmed(int id)
        {
            var hm = db.HelpMenus.Single(p => p.MenuId == id);
            db.HelpMenus.DeleteObject(hm);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult DocList(int id)
        {
            DocMgrVM vm = new DocMgrVM();
            vm.HelpMenu = db.HelpMenus.Single(p => p.MenuId == id);
            vm.HelpDocs = db.HelpDocs.Where(p => p.MenuId == id);
            return View(vm);
        }

        public ActionResult DocNew(int id)
        {
            DocMgrVM vm = new DocMgrVM();
            vm.HelpMenu = db.HelpMenus.Single(p => p.MenuId == id);
            return View(vm);
        }

        [HttpPost]
        public ActionResult DocNew(HttpPostedFileBase uploadFile, DocMgrVM vm, int id)
        {
            if (ModelState.IsValid)
            {
                if (uploadFile != null && uploadFile.ContentLength != 0)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        HelpDoc doc = new HelpDoc();
                        doc.DocId = vm.HelpDoc.DocId;
                        doc.MenuId = id;
                        doc.DocName = vm.HelpDoc.DocName;
                        doc.Description = vm.HelpDoc.Description;
                        doc.DocInfo = vm.HelpDoc.DocInfo;
                        doc.IsVisible = vm.HelpDoc.IsVisible;

                        uploadFile.InputStream.CopyTo(ms);
                        byte[] fileData = ms.GetBuffer();
                        doc.Filename = uploadFile.FileName;
                        doc.ContentLength = uploadFile.ContentLength;
                        doc.ContentType = uploadFile.ContentType;
                        doc.Data = fileData;

                        db.HelpDocs.AddObject(doc);
                        db.SaveChanges();

                        return RedirectToAction("DocList", new { id = id });
                    }
                }
                else
                {
                    ModelState.AddModelError("", "File harus diisi");
                }
            }
            vm.HelpMenu = db.HelpMenus.Single(p => p.MenuId == id);
            return View(vm);
        }

        public ActionResult DocEdit(int id)
        {
            DocMgrVM vm = new DocMgrVM();
            vm.HelpDoc = db.HelpDocs.Single(p => p.DocId == id);
            return View(vm);
        }

        [HttpPost]
        public ActionResult DocEdit(HttpPostedFileBase uploadFile, DocMgrVM vm, int id)
        {
            if (ModelState.IsValid)
            {
                HelpDoc doc = db.HelpDocs.Single(p => p.DocId == id);
                doc.DocName = vm.HelpDoc.DocName;
                doc.Description = vm.HelpDoc.Description;
                doc.DocInfo = vm.HelpDoc.DocInfo;
                doc.IsVisible = vm.HelpDoc.IsVisible;

                if (uploadFile != null && uploadFile.ContentLength != 0)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        uploadFile.InputStream.CopyTo(ms);
                        byte[] fileData = ms.GetBuffer();
                        doc.Filename = uploadFile.FileName;
                        doc.ContentLength = uploadFile.ContentLength;
                        doc.ContentType = uploadFile.ContentType;
                        doc.Data = fileData;
                    }
                }
                db.SaveChanges();

                return RedirectToAction("DocList", new { id = vm.HelpDoc.MenuId });
            }
            vm.HelpDoc = db.HelpDocs.Single(p => p.DocId == id);
            return View(vm);
        }

        public ActionResult DocDownload(int id)
        {
            byte[] fileData;
            var doc = db.HelpDocs.Single(p => p.DocId == id);
            fileData = doc.Data.ToArray();
            return File(fileData, doc.ContentType, doc.Filename);
        }
    }
}
