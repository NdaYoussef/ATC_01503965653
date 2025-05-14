using EventManagmentTask.DTOs;
using EventManagmentTask.DTOs.EventDTO;

namespace EventManagmentTask.Interfaces
{
    public interface IEventRepository
    {
        Task<ResponseDto> GetAllEvents();
        Task<ResponseDto> GetEventsById(int id);
        Task<ResponseDto> AddEvent(EventDto eventDto);
        Task<ResponseDto> EditEvent(EventDto eventDto ,int id );
        Task<ResponseDto> DeleteEvent(int id);
    }
}
