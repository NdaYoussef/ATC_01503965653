using EventManagmentTask.DTOs;
using EventManagmentTask.DTOs.UserDto;
using EventManagmentTask.Models;
using Mapster;

namespace EventManagmentTask.Mapping
{
    public class MapsterConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            // RegisterDto =>  User
            config.NewConfig<RegisterDto, User>();


            // LoginDto => User 
            config.NewConfig<LoginDto, User>();
            config.NewConfig<EventDto, Event>();

        }
    }
}
