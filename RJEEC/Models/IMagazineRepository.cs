using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RJEEC.Models
{
    public interface IMagazineRepository
    {
        IEnumerable<Magazine> GetAllMagazines();
        IEnumerable<Magazine> GetLast5Magazines();
        Magazine GetMagazine(int id);
        Magazine GetMagazineByVolumeNumberYear(int? volume, int? number, int? year);
        Magazine AddMagazine(Magazine magazine);
        Magazine UpdateMagazine(Magazine magazineChanges);
    }
}
