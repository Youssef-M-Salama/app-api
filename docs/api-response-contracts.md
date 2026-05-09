# API Response Contracts
> Charity-Donor Platform — V1

---

## SUCCESS RESPONSES

### Pattern 1: Simple Success (No Data)
Used in: Logout, Delete, Activate, Deactivate, Accept, Reject, Fulfill
HTTP Status: `200 OK` or `204 No Content`

```json
{
  "success": true,
  "message": "Operation completed successfully",
  "timestamp": "2024-01-15T10:30:00Z"
}
```

---

### Pattern 2: Success with Object
Used in: Login, Register, Refresh, Create, Get Details, Update
HTTP Status: `200 OK` (GET, PUT) or `201 Created` (POST)

```json
{
  "success": true,
  "message": "CharityNeed created successfully",
  "data": {
    "charityNeedId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "productName": "Rice",
    "quantity": 100.5,
    "unit": "Kg",
    "status": "Pending",
    "createdAt": "2024-01-15T10:30:00Z"
  },
  "timestamp": "2024-01-15T10:30:00Z"
}
```

---

### Pattern 3: Success with Array + Pagination
Used in: Browse CharityNeeds, Browse Offers, Get List
HTTP Status: `200 OK`

```json
{
  "success": true,
  "message": "CharityNeeds retrieved successfully",
  "data": [
    {
      "charityNeedId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "productName": "Rice",
      "quantity": 100.0,
      "unit": "Kg",
      "category": "Food",
      "priority": "Urgent"
    }
  ],
  "pagination": {
    "page": 1,
    "pageSize": 10,
    "totalPages": 5,
    "totalCount": 50,
    "hasNext": true,
    "hasPrevious": false
  },
  "timestamp": "2024-01-15T10:30:00Z"
}
```

---

### Pattern 4: Success with Stats / Dashboard
Used in: Dashboard, Statistics
HTTP Status: `200 OK`

```json
{
  "success": true,
  "message": "Dashboard statistics retrieved successfully",
  "data": {
    "totalCharityNeeds": 15,
    "pendingApproval": 2,
    "activeCharityNeeds": 10,
    "needApplicationsReceived": 8
  },
  "timestamp": "2024-01-15T10:30:00Z"
}
```

---

## ERROR RESPONSES

All errors use this pattern:

```json
{
  "success": false,
  "message": "Human-readable error message",
  "error": {
    "code": "ERROR_CODE",
    "details": {}
  },
  "timestamp": "2024-01-15T10:30:00Z"
}
```

### Validation Error (400)
```json
{
  "success": false,
  "message": "Validation failed",
  "error": {
    "code": "VALIDATION_ERROR",
    "details": {
      "errors": [
        { "field": "Email", "message": "Email is already registered" },
        { "field": "ConfirmPassword", "message": "Password and confirm password do not match" }
      ]
    }
  },
  "timestamp": "2024-01-15T10:30:00Z"
}
```

---

## ERROR CODES REFERENCE

| Error Code (enum value) | HTTP Status | Description |
|------------------------|-------------|-------------|
| `VALIDATION_ERROR` | 400 Bad Request | Input validation failed or identity errors |
| `REQUIRED_FIELD_MISSING` | 400 Bad Request | Required field not provided |
| `INVALID_FORMAT` | 400 Bad Request | Field value has invalid format |
| `INVALID_PAGE` | 400 Bad Request | Page number is <= 0 |
| `INVALID_PAGE_SIZE` | 400 Bad Request | Page size is <= 0 |
| `PAGE_SIZE_LIMIT_EXCEEDED` | 400 Bad Request | Page size exceeds max (50) |
| `UNAUTHORIZED` | 401 Unauthorized | Not logged in or token invalid/expired |
| `INVALID_CREDENTIALS` | 401 Unauthorized | Wrong username/password |
| `TOKEN_EXPIRED` | 401 Unauthorized | JWT or refresh token expired |
| `FORBIDDEN` | 403 Forbidden | Authenticated but lacks permission |
| `ACCOUNT_NOT_VERIFIED` | 403 Forbidden | Email not confirmed |
| `ACCOUNT_INACTIVE` | 403 Forbidden | User account deactivated |
| `NOT_FOUND` | 404 Not Found | Resource does not exist |
| `ALREADY_EXISTS` | 409 Conflict | Duplicate entry (email, username, etc.) |
| `OPERATION_NOT_ALLOWED` | 422 Unprocessable | Business logic prevents action (e.g. fulfilled state) |
| `INVALID_STATUS` | 422 Unprocessable | Entity is in wrong status for operation |
| `RATE_LIMIT_EXCEEDED` | 429 Too Many | Sliding window limit reached |
| `INTERNAL_SERVER_ERROR` | 500 Internal Error | Unexpected server-side error |
| `DATABASE_ERROR` | 500 Internal Error | Database-related failure |

---

## ENUM REFERENCE

### ProductCategory
`0: Food`, `1: Clothing`, `2: Medical`, `3: Education`, `4: Other`

### MeasurementUnit
`0: Ton`, `1: Kg`, `2: Gram`, `3: Liter`, `4: Ml`, `5: Pack`, `6: Box`, `7: Can`, `8: Piece`

### CharityNeedPriority
`0: Urgent`, `1: High`, `2: Normal`, `3: Low`

### CharityNeedStatus / OfferStatus
`0: Pending`, `1: Approved`, `2: Rejected`, `3: Fulfilled`, `4: Expired`