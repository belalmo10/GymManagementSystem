using AutoMapper;
using GymManagement.BLL.Services.ViewModels.MemberViewModel;
using GymManagement.BLL.Services.ViewModels.PlanViewModel;
using GymManagement.BLL.Services.ViewModels.SessionViewModel;
using GymManagement.DAL.Data.Models;

namespace GymManagement.BLL
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            MapMember();
            MapSession();
            MapPlan();
        }


        private void MapMember()
        {
            CreateMap<Member , MemberViewModel>()
                .ForMember(des => des.Address , opt => opt.MapFrom(src => $"{src.Address.Street} - {src.Address.BuildingNumber} - {src.Address.City}"))
                .ForMember(des => des.DateOfBirth , opt => opt.MapFrom(src => src.DateOfBirth.ToShortDateString()));

            CreateMap<HealthRecord , HealthRecordViewModel>().ReverseMap();

            CreateMap<Member , MemberToUpdateViewModel>()
                .ForMember(des => des.BuildingNumber , opt => opt.MapFrom(src => src.Address.BuildingNumber))
                .ForMember(des => des.Street , opt => opt.MapFrom(src => src.Address.Street))
                .ForMember(des => des.City , opt => opt.MapFrom(src => src.Address.City));

            CreateMap<MemberToUpdateViewModel , Member>()
                .ForMember(des => des.Name , opt => opt.Ignore())
                .ForMember(des => des.Photo , opt => opt.Ignore())

                .AfterMap((src , des) =>
                {
                    des.Address.BuildingNumber = src.BuildingNumber;
                    des.Address.Street = src.Street;
                    des.Address.City = src.City;
                });


            CreateMap<CreateMemberViewModel , Member>()
                .ForMember(src => src.Address , opt => opt.MapFrom(src => new Address
                {
                    BuildingNumber = src.BuildingNumber ,
                    Street = src.Street ,
                    City = src.City ,
                }))
                .ForMember(des => des.HealthRecord , opt => opt.MapFrom(src => src.HealthRecordViewModel));
        }
        private void MapSession()
        {
            CreateMap<CreateSessionViewModel , Session>();
            CreateMap<Trainer , TrainerSelectedViewModel>();
            CreateMap<Category , CategorySelectedViewModel>();
            CreateMap<Session , SessionViewModel>()
                .ForMember(des => des.TrainerName , opt => opt.MapFrom(src => src.Trainer.Name))
                .ForMember(des => des.CategoryName , opt => opt.MapFrom(src => src.Category.CategoryName));

            CreateMap<Session , UpdateSessionViewModel>().ReverseMap();

        }
        private void MapPlan()
        {
            CreateMap<Plan , PlanViewModel>().ReverseMap();
            CreateMap<Plan , UpdatePlanViewModel>().ReverseMap();

        }
    }
}
