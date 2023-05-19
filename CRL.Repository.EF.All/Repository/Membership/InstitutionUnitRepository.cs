using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.UnitOfWork;
using CRL.Model.ModelViews;

using CRL.Respositoy.EF.All;
using CRL.Respositoy.EF.All.Common.Repository;
using CRL.Model.Messaging;
using CRL.Model.Memberships;
using CRL.Model.Memberships.IRepository;
using CRL.Model.ModelViews.Memberships;


namespace CRL.Repository.EF.All.Repository.Memberships
{
    public class InstitutionUnitRepository : Repository<InstitutionUnit, int>, IInstitutionUnitRepository
    {
        public InstitutionUnitRepository(IUnitOfWork uow)
            : base(uow)
        {
        }


        public override string GetEntitySetName()
        {
            return "InstitutionUnit";
        }

        public InstitutionUnit GetUnitDetailById(int id)
        {
            return ctx.InstitutionUnits.Include("People").Where(s => s.Id == id).Single();
        }

        public IQueryable<InstitutionUnit> GetDbSetComplete()
        {
            throw new NotImplementedException();
        }

        public ViewInstitutionUnitsResponse UnitsGridView(ViewInstitutionUnitsRequest request)
        {
            ViewInstitutionUnitsResponse response = new ViewInstitutionUnitsResponse();

            
            var query = ctx.InstitutionUnits.AsNoTracking().Where(s => s.IsDeleted == false);

            if (!(String.IsNullOrEmpty(request.Name)))
            {
                query = query.Where(s => s.Name == request.Name);
            }
            if (request.InstitutionId > 0)
            {
                query = query.Where(s => s.InstitutionId == request.InstitutionId);
            }

              var query2 = query.Select(s => new InstitutionUnitGridView
            {
                Id = s.Id,
                 Name = s.Name,
                 Email = s.Email,
                  CreatedOn = s.CreatedOn ,
                   HasUsers = s.People.Where(q=>q.IsDeleted==false ).Count ()>0,
                   IsActive = s.IsActive 
                    
            });


              if (String.IsNullOrWhiteSpace(request.SortColumn) == false)
              {

                  if (request.SortColumn == "Name")
                  {
                      if (request.SortOrder == "desc")
                      {
                          query2 = query2.OrderByDescending(s => s.Name );
                      }
                      else
                      {
                          query2 = query2.OrderBy(s => s.Name);
                      }
                  }
              }
              else
              {
                  query2 = query2.OrderBy(s => s.Name);
              }

              if (request.PageIndex > 0)
              {
                  ////We are doing clientside filtering            
                  query2 = query2.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize);

              }


              response.InstitutionUnitGridView  = query2.ToList();
              return response;
            
        }

       
    }
}
