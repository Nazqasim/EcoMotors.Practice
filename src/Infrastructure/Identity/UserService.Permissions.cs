﻿using EcoMotorsPractice.Application.Common.Caching;
using EcoMotorsPractice.Application.Common.Exceptions;
using EcoMotorsPractice.Shared.Authorization;
using Microsoft.EntityFrameworkCore;

namespace EcoMotorsPractice.Infrastructure.Identity;

internal partial class UserService
{
    public async Task<List<string>> GetPermissionsAsync(string userId, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(userId);

        _ = user ?? throw new NotFoundException(_localizer["User Not Found."]);

        var userRoles = await _userManager.GetRolesAsync(user);
        var permissions = new List<string>();
        foreach (var role in await _roleManager.Roles
            .Where(r => userRoles.Contains(r.Name))
            .ToListAsync(cancellationToken))
        {
            permissions.AddRange(await _db.RoleClaims
                .Where(rc => rc.RoleId == role.Id && rc.ClaimType == AppClaims.Permission)
                .Select(rc => rc.ClaimValue)
                .ToListAsync(cancellationToken));
        }

        return permissions.Distinct().ToList();
    }

    public async Task<bool> HasPermissionAsync(string userId, string permission, CancellationToken cancellationToken)
    {
        var permissions = await _cache.GetOrSetAsync(
             _cacheKeys.GetCacheKey(AppClaims.Permission, userId),
             () => GetPermissionsAsync(userId, cancellationToken),
             cancellationToken: cancellationToken);

        return permissions?.Contains(permission) ?? false;
    }

    public Task InvalidatePermissionCacheAsync(string userId, CancellationToken cancellationToken) =>
        _cache.RemoveAsync(_cacheKeys.GetCacheKey(AppClaims.Permission, userId), cancellationToken);
}