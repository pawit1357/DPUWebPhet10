using System;
using System.Linq;

namespace DPUWebPhet10.Models
{
    public class DPUPhet10Repository : IRepository
    {
        private ChinaPhet10Entities context = new ChinaPhet10Entities();

        //public IQueryable<TB_M_TITLE> GetTitles()
        //{
        //    return context.TB_M_TITLE.Where( a=>a.TITLE_LANGUAGE=="TH" );
        //}

        //public IQueryable<TB_M_LEVEL> GetLevels()
        //{
        //    return context.TB_M_LEVEL.Where(a => a.LEVEL_LANGUAGE == "TH");
        //}

        public IQueryable<TB_M_PROVINCE> GetProvinces()
        {
            return context.TB_M_PROVINCE;
        }

        public IQueryable<TB_M_AMPHUR> GetAmphur(string PROVINCE_ID)
        {
            int provinceId = Convert.ToInt16(PROVINCE_ID);
            return context.TB_M_AMPHUR.Where(a => a.PROVINCE_ID == provinceId);
        }

        public IQueryable<TB_M_DISTRICT> GetDistrict(string AMPHUR_ID)
        {
            int amphurId = Convert.ToInt16(AMPHUR_ID);
            return context.TB_M_DISTRICT.Where(a => a.AMPHUR_ID == amphurId);
        }

        public IQueryable<TB_ROOM> GetRoomForLevel(string ROOM_FOR_LEVEL)
        {
            int levelId = Convert.ToInt16(ROOM_FOR_LEVEL);
            return context.TB_ROOM.Where(a => a.ROOM_FOR_LEVEL == levelId);
        }

    }
}