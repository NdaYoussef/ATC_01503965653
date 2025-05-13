using EventManagmentTask.DTOs;

namespace EventManagmentTask.Interfaces
{
    public interface IEventRepository
    {
        Task<ResponseDto> GetAllEvents();
        Task<ResponseDto> GetEventsById(int id);
        Task<ResponseDto> AddEvent(EventDto eventDto);
        Task<ResponseDto> EditEvent(EventDto eventDto);
        Task<ResponseDto> DeleteEvent(int id);
    }
}
