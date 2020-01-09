using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RJEEC.Models
{
    public class MagazineRepository : IMagazineRepository
    {
        private readonly RJEECDbContext context;

        public MagazineRepository(RJEECDbContext context)
        {
            this.context = context;
        }
        public Magazine AddMagazine(Magazine magazine)
        {
            context.Magazines.Add(magazine);
            context.SaveChanges();
            return magazine;
        }

        public IEnumerable<Magazine> GetAllMagazines()
        {
            return context.Magazines;
        }

        public IEnumerable<Magazine> GetLast5Magazines()
        {
            return context.Magazines.OrderByDescending(m => m.PublishingYear).ThenByDescending(m => m.Volume).ThenByDescending(m => m.Number).Take(5);
        }

        public Magazine GetMagazine(int id)
        {
            return context.Magazines.Find(id);
        }

        public Magazine GetMagazineByVolumeNumberYear(int? volume, int? number, int? year)
        {
            return context.Magazines.FirstOrDefault(m => m.Volume == volume && m.Number == number && m.PublishingYear == year);
        }
    }
}
