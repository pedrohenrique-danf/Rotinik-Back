using Rotinik.Features.Medals.DTOs;

namespace Rotinik.Features.Medals;

public static class MedalMappingExtensions
{
    public static IQueryable<UserMedalResponseDto> MapToUserMedalDto(this IQueryable<UserMedal> query)
    {
        return query.Select(um => new UserMedalResponseDto
        {
            MedalId = um.MedalId,
            AchievedAt = um.AchievedAt,
            IsEquipped = um.IsEquipped,
            Medal = new MedalResponseDto
            {
                Id = um.Medal.Id,
                Name = um.Medal.Name,
                Description = um.Medal.Description,
                IconUrl = um.Medal.IconUrl,
                TriggerType = um.Medal.TriggerType,
                TargetValue = um.Medal.TargetValue,
                RewardPoints = um.Medal.RewardPoints,
                RewardCoins = um.Medal.RewardCoins
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
            TriggerType = m.TriggerType,
            TargetValue = m.TargetValue,
            RewardPoints = m.RewardPoints,
            RewardCoins = m.RewardCoins
        });
    }
}