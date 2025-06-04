using api.Dtos.Activity;
using api.Models;
using System.Reflection.Metadata.Ecma335;

namespace api.Mappers{

    public static class ActivityMappers
    {
        public static ActivityDto toDto(this Activity activity){

            return new ActivityDto{
                id = activity.id,
                identifyActivity = activity.identifyActivity,
                activityName = activity.activityName,
                activityType = activity.activityType,
                creationDate = activity.creationDate,
                startDate = activity.startDate,
                endDate = activity.endDate,
                activityStatus = activity.activityStatus,
                urlSources = activity.urlSources,
                userId = activity.userId                
            };
        }

        public static Activity ToActivityFromCreateDto(this CreateActivityRequestDto activityRequestDto){
            return new Activity{
                identifyActivity = activityRequestDto.identifyActivity,
                activityName = activityRequestDto.activityName,
                activityType = activityRequestDto.activityType,
                creationDate = activityRequestDto.creationDate,
                startDate = activityRequestDto.startDate,
                endDate = activityRequestDto.endDate,
                activityStatus = activityRequestDto.activityStatus,
                urlSources = activityRequestDto.urlSources,
                userId = activityRequestDto.userId
            };
        }
        
        public static void ToActivityFromUpdateDto (this Activity activity, UpdateActivityRequestDto activityRequestDto){

            if (activity == null) {
                throw new ArgumentNullException(nameof(activity));
            }
            if (activityRequestDto == null){
                throw new ArgumentNullException(nameof(activityRequestDto));
            }
            activity.startDate = activityRequestDto.startDate;
            activity.endDate = activityRequestDto.endDate;
            activity.activityStatus = activityRequestDto.activityStatus;
        }
    }
}