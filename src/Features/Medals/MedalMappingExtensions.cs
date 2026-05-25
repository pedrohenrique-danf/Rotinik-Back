using Rotinik.Features.Medals.DTOs;

namespace Rotinik.Features.Medals;

public static class MedalMappingExtensions
{
    public static IQueryable<UserMedalResponseDto> MapToUserMedalDto(this IQueryable<UserMedal> query)
    {
        return query.Select(um => new UserMedalResponseDto
        {
            AchievedAt = um.AchievedAt,
            Medal = new MedalResponseDto
            {
                Id = um.Medal.Id,
                Name = um.Medal.Name,
                Description = um.Medal.Description,
                IconUrl = um.Medal.IconUrl,
                PointsThreshold = um.Medal.PointsThreshold
            }
        });
    }

    public static IQueryable<MedalResponseDto> MapToMedalDto(this IQueryable<Medal> query)
    {
        return query.Select(m => new MedalResponseDto
        {
            Id = m.Id,
            Name = m.Name,
            Description = m.Description,
            IconUrl = m.IconUrl,
            PointsThreshold = m.PointsThreshold
        });
    }
}