
namespace DPUWebPhet10.Models
{
    interface IRepository
    {
        //System.Linq.IQueryable<TB_M_TITLE> GetTitles();
        //System.Linq.IQueryable<TB_M_LEVEL> GetLevels();
        System.Linq.IQueryable<TB_M_PROVINCE> GetProvinces();
        System.Linq.IQueryable<TB_M_AMPHUR> GetAmphur(string PROVINCE_ID);
        System.Linq.IQueryable<TB_M_DISTRICT> GetDistrict(string AMPHUR_ID);

        System.Linq.IQueryable<TB_ROOM> GetRoomForLevel(string ROOM_FOR_LEVEL);
    
    }
}
