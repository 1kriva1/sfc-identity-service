﻿namespace SFC.Identity.Application.Common.Exceptions;

public class AuthorizationException : Exception
{
    public AuthorizationException(string message) : base(message) { }
}
