﻿using System.Security.Claims;

namespace RunGroupWebApp
{
    public static class ClaimPrincipalExtensions
    {
        public static string GetUserId(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.NameIdentifier).Value;

        }


    }
}
