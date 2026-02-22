namespace App.Core.Enums
{
    /// <summary>
    /// Standardized error codes used across the application.
    /// Helps frontend and logging systems handle errors consistently.
    /// </summary>
    public enum ErrorCode
    {
        /// <summary>Generic validation error.</summary>
        VALIDATION_ERROR,

        /// <summary>Required field missing.</summary>
        REQUIRED_FIELD_MISSING,

        /// <summary>Invalid format.</summary>
        INVALID_FORMAT,

        /// <summary>User is not authenticated.</summary>
        UNAUTHORIZED,

        /// <summary>Invalid login credentials.</summary>
        INVALID_CREDENTIALS,

        /// <summary>Token expired.</summary>
        TOKEN_EXPIRED,

        /// <summary>User is authenticated but not allowed.</summary>
        FORBIDDEN,

        /// <summary>Account not verified.</summary>
        ACCOUNT_NOT_VERIFIED,

        /// <summary>Account inactive.</summary>
        ACCOUNT_INACTIVE,

        /// <summary>Resource not found.</summary>
        NOT_FOUND,

        /// <summary>Entity already exists.</summary>
        ALREADY_EXISTS,

        /// <summary>Operation not allowed by business rules.</summary>
        OPERATION_NOT_ALLOWED,

        /// <summary>Invalid state/status.</summary>
        INVALID_STATUS,

        /// <summary>Unexpected server error.</summary>
        INTERNAL_SERVER_ERROR,

        /// <summary>Database related error.</summary>
        DATABASE_ERROR,
        /// <summary>Pagination page is invalid.</summary>
        INVALID_PAGE,

        /// <summary>Pagination page size is invalid.</summary>
        INVALID_PAGE_SIZE,

        /// <summary>Pagination limit exceeded.</summary>
        PAGE_SIZE_LIMIT_EXCEEDED
    }
}