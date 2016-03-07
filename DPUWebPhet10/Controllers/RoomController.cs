using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DPUWebPhet10.Models;

namespace DPUWebPhet10.Controllers
{
    public class RoomController : Controller
    {
        private ChinaPhet10Entities db = new ChinaPhet10Entities();

        //
        // GET: /Room/

        public ActionResult Index()
        {
            var tb_room = db.TB_ROOM.Include("TB_M_BUILDING").Include("TB_M_LEVEL");
            return View(tb_room.ToList());
        }

        //
        // GET: /Room/Details/5

        public ActionResult Details(decimal id = 0)
        {
            TB_ROOM tb_room = db.TB_ROOM.Single(t => t.ROOM_ID == id);
            if (tb_room == null)
            {
                return HttpNotFound();
            }
            return View(tb_room);
        }

        //
        // GET: /Room/Create

        public ActionResult Create()
        {
            ViewBag.ROOM_BUILD = new SelectList(db.TB_M_BUILDING, "BUILDING_ID", "BUILDING_NAME");
            ViewBag.ROOM_FOR_LEVEL = new SelectList(db.TB_M_LEVEL, "LEVEL_ID", "LEVEL_NAME_TH");
            return View();
        }

        //
        // POST: /Room/Create

        [HttpPost]
        public ActionResult Create(TB_ROOM tb_room)
        {
            if (ModelState.IsValid)
            {
                tb_room.ROOM_FLOOR = 0;
                db.TB_ROOM.Add(tb_room);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ROOM_BUILD = new SelectList(db.TB_M_BUILDING, "BUILDING_ID", "BUILDING_NAME", tb_room.ROOM_BUILD);
            ViewBag.ROOM_FOR_LEVEL = new SelectList(db.TB_M_LEVEL, "LEVEL_ID", "LEVEL_NAME_TH", tb_room.ROOM_FOR_LEVEL);
            return View(tb_room);
        }

        //
        // GET: /Room/Edit/5

        public ActionResult Edit(decimal id = 0)
        {
            TB_ROOM tb_room = db.TB_ROOM.Single(t => t.ROOM_ID == id);
            if (tb_room == null)
            {
                return HttpNotFound();
            }
            ViewBag.ROOM_BUILD = new SelectList(db.TB_M_BUILDING, "BUILDING_ID", "BUILDING_NAME", tb_room.ROOM_BUILD);
            ViewBag.ROOM_FOR_LEVEL = new SelectList(db.TB_M_LEVEL, "LEVEL_ID", "LEVEL_NAME_TH", tb_room.ROOM_FOR_LEVEL);
            return View(tb_room);
        }

        //
        // POST: /Room/Edit/5

        [HttpPost]
        public ActionResult Edit(TB_ROOM tb_room)
        {
            if (ModelState.IsValid)
            {
                tb_room.ROOM_FLOOR = 0;

                var _update = db.TB_ROOM.FirstOrDefault(f => f.ROOM_ID == tb_room.ROOM_ID);
                if (_update != null)
                {
                    //ROOM_ID              =ROOM_ID 
                    _update.ROOM_FOR_LEVEL = tb_room.ROOM_FOR_LEVEL;
                    _update.ROOM_BUILD = tb_room.ROOM_BUILD;
                    _update.ROOM_FLOOR = tb_room.ROOM_FLOOR;
                    _update.ROOM_NUMBER = tb_room.ROOM_NUMBER;
                    _update.ROOM_SEAT_COUNT = tb_room.ROOM_SEAT_COUNT;
                    _update.ROOM_COMMITTEE_COUNT = tb_room.ROOM_COMMITTEE_COUNT;
                    _update.ROOM_ROW = tb_room.ROOM_ROW;
                    _update.ROW_1 = tb_room.ROW_1;
                    _update.ROW_2 = tb_room.ROW_2;
                    _update.ROW_3 = tb_room.ROW_3;
                    _update.ROW_4 = tb_room.ROW_4;
                    _update.ROW_5 = tb_room.ROW_5;
                    _update.ROW_6 = tb_room.ROW_6;
                    _update.ROW_7 = tb_room.ROW_7;
                    _update.ROW_8 = tb_room.ROW_8;
                    _update.ROW_9 = tb_room.ROW_9;
                    _update.ROW_10 = tb_room.ROW_10;
                    _update.ROW_11 = tb_room.ROW_11;
                    _update.ROW_12 = tb_room.ROW_12;
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ROOM_BUILD = new SelectList(db.TB_M_BUILDING, "BUILDING_ID", "BUILDING_NAME", tb_room.ROOM_BUILD);
            ViewBag.ROOM_FOR_LEVEL = new SelectList(db.TB_M_LEVEL, "LEVEL_ID", "LEVEL_NAME_TH", tb_room.ROOM_FOR_LEVEL);
            return View(tb_room);
        }

        //
        // GET: /Room/Delete/5

        public ActionResult Delete(decimal id = 0)
        {
            TB_ROOM tb_room = db.TB_ROOM.Single(t => t.ROOM_ID == id);
            if (tb_room == null)
            {
                return HttpNotFound();
            }
            return View(tb_room);
        }

        //
        // POST: /Room/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(decimal id)
        {
            TB_ROOM tb_room = db.TB_ROOM.Single(t => t.ROOM_ID == id);
            db.TB_ROOM.Remove(tb_room);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}