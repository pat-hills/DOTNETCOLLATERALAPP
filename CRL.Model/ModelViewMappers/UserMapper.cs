using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CRL.Infrastructure.Helpers;
using CRL.Model.ModelViews;
using CRL.Service.Common;
using CRL.Model.ModelViews.Memberships;
using CRL.Model.Memberships;

namespace CRL.Model.ModelViewMappers
{
    public static class UserMapper
    {
        public static ICollection<UserView> ConvertToUserViews(
                                                this ICollection<User> Users)
        {
            List<UserView> iview = new List<UserView>();
            foreach (var usr in Users)
            {
                iview.Add(usr.ConvertToUserView());
            }

            return iview;
        }

        public static UserView ConvertToUserView(this User User)
        {
            UserView UserView = new UserView()
            {
                 Address = User.Address .Address ,
                  City = User.Address .City,                   
                    CountryId = User.CountryId ,
                     CountyId = User.CountyId ,                    
                       Email = User.Address .Email ,
                        FirstName = User.FirstName ,
                         MiddleName = User.MiddleName ,
                          Surname = User.Surname ,
                           Fullname =NameHelper.GetFullName(User.FirstName, User.MiddleName, User.Surname),
                            Gender = User.Gender ,                            
                              InstitutionId = User.InstitutionId ,
                               PersonTitle = User.Title ,
                                Id = User.Id ,
                                 Username = User .Username ,
                                  Phone = User.Address .Phone ,
                                   NationalityId = User.NationalityId ,
                                    isPayPointUser = User.isPayPointUser ,
                                    InstitutionUnitId = User .InstitutionUnitId ,
                 InstitutionUnit = User.InstitutionUnitId != null ? User.InstitutionUnit.Name : ""
                                     
 
            };

            if (User.Nationality !=null)
            {
                UserView.Nationality =  User.Nationality.Name ;
                

            }
            if (User.Country != null)
            {
                UserView.Country = User.Country.Name;
            }
            if (User.County != null)
            {
                UserView.County = User.County.Name;
            }
           


          
          
            return UserView;
        }

        //public static ICollection<User> ConvertToUsers(
        //                                       this ICollection<UserView> UserViews)
        //{
        //    return Mapper.Map<ICollection<UserView>,
        //                      ICollection<User>>(UserViews);
        //}

        public static UserView ConvertToUserView(this MembershipRegistrationView membershipRegView)
        {
            UserView userView = new UserView()
            {


                Address = membershipRegView.Address.TrimNull(),
                City = membershipRegView.City.TrimNull(),
                CountryId = membershipRegView.CountryId,
                CountyId = membershipRegView.CountyId,
                Email = membershipRegView.Email.Trim(),
                FirstName = membershipRegView.FirstName.Trim(),
                MiddleName = membershipRegView.MiddleName.TrimNull(),
                Surname = membershipRegView.Surname.Trim(),
                NationalityId = membershipRegView.NationalityId,
                Password = membershipRegView.Password.Trim(),
                PersonTitle = membershipRegView.Title,
                Username = membershipRegView.AccountName.Trim(),
                Gender = membershipRegView.Gender,
                Phone = membershipRegView.Phone.TrimNull()
            };

            return userView;


        }

        public static User ConvertToUser(this UserView UserView)
        {
            User c = new User();          
            UserView.ConvertToUser(c);
            return c;
           
            
        }

        public static void ConvertToUser(this UserView iview, User model)
        {
            model.Username = iview.Username.TrimNull ();
            model.FirstName = iview.FirstName.TrimNull(); ;
            model.MiddleName = iview.MiddleName.TrimNull(); ;
            model.Surname = iview.Surname.TrimNull(); ;
            model.Title = iview.PersonTitle;

            model.Address.Email = iview.Email.TrimNull(); ;
            if (iview.InstitutionId == null)
            {
                model.CountryId = iview.CountryId;
                model.CountyId = iview.CountyId;
                model.NationalityId = iview.NationalityId;
                model.Address.City = iview.City;
                model.Address.Address = iview.Address.TrimNull(); ;
                model.Address.Phone = iview.Phone.TrimNull(); ;
            }
            model.Gender = iview.Gender;
            model.InstitutionId = iview.InstitutionId;
            model.InstitutionUnitId = iview.InstitutionUnitId;
            model.isPayPointUser = iview.isPayPointUser;
           
            
           
        }


        //public static IEnumerable<UserGridView> ConvertToUserGridView(
        //                                       this IEnumerable<User> Users)
        //{
          
        //    ICollection<UserGridView> iviews = new List<UserGridView>();
        //    foreach (var user  in Users)
        //    {
        //        iviews.Add(user.ConvertToUserGridView());
        //    }

        //    return iviews;
            
        //}

        //public static UserGridView ConvertToUserGridView(this User User)
        //{
        //    UserGridView UserGridView = new UserGridView();
        //    Mapper.Map<User, UserGridView>(User, UserGridView);
        //    UserGridView.FullName = NameHelper.GetFullName(User.FirstName, User.MiddleName, User.Surname);
        //    UserGridView .Phone =User.Address.Phone;       
        //    UserGridView.Email = User.Address.Email;
        //    UserGridView.isPayPoint = User.isPayPointUser;

             
        //     return UserGridView;

        //}
    }
}
