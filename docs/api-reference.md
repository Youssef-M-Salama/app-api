# API Reference
> Charity-Donor Platform — V1

---

## AUTHENTICATION APIs
**Prefix**: `/api/v1/auth`

| # | Method | Endpoint | Auth | Description | Payload |
|---|--------|----------|------|-------------|---------|
| 1 | POST | `/register` | No | Register new user | JSON |
| 2 | POST | `/login` | No | User login | JSON |
| 3 | POST | `/refresh` | No | Refresh access token | JSON |
| 4 | POST | `/logout` | Yes | User logout | - |
| 5 | GET | `/verify-email` | No | Verify email with token | Query |
| 6 | POST | `/resend-verification` | No | Resend verification email | JSON |

---

## PUBLIC APIs (Guest Access)
**Prefix**: `/api/v1/public`

| # | Method | Endpoint | Auth | Description | Payload |
|---|--------|----------|------|-------------|---------|
| 7 | GET | `/statistics` | No | Platform statistics | - |
| 8 | GET | `/charity-needs` | No | Browse approved needs | Query |
| 9 | GET | `/offers` | No | Browse available offers | Query |
| 10 | GET | `/charity-needs/{charityNeedId}` | No | Charity need details | - |
| 11 | GET | `/offers/{offerId}` | No | Offer details | - |

---

## ADMIN APIs
**Prefix**: `/api/v1/admin`

| # | Method | Endpoint | Auth | Description | Payload |
|---|--------|----------|------|-------------|---------|
| 12 | GET | `/dashboard` | Admin | Dashboard statistics | - |
| 13 | GET | `/verifications/pending` | Admin | Pending verifications | - |
| 14 | PATCH | `/verifications/verify` | Admin | Verify user account | JSON |
| 15 | GET | `/charity-needs/pending` | Admin | Pending charity needs | Query |
| 16 | PATCH | `/charity-needs/approve` | Admin | Approve charity need | JSON |
| 17 | PATCH | `/charity-needs/reject` | Admin | Reject charity need | JSON |
| 18 | GET | `/offers/pending` | Admin | Pending donor offers | Query |
| 19 | PATCH | `/offers/approve` | Admin | Approve donor offer | JSON |
| 20 | PATCH | `/offers/reject` | Admin | Reject donor offer | JSON |
| 21 | GET | `/users` | Admin | All users with filters | Query |
| 22 | PATCH | `/users/deactivate` | Admin | Deactivate user account | JSON |
| 23 | PATCH | `/users/activate` | Admin | Activate user account | JSON |

---

## CHARITY APIs
**Prefix**: `/api/v1/charity`

| # | Method | Endpoint | Auth | Description | Payload |
|---|--------|----------|------|-------------|---------|
| 24 | GET | `/dashboard` | Charity | Dashboard statistics | - |
| 25 | POST | `/charity-needs` | Charity | Create charity need | Multipart |
| 26 | GET | `/charity-needs` | Charity | My charity needs | Query |
| 27 | GET | `/charity-needs/{charityNeedId}` | Charity | Charity need details | - |
| 28 | PUT | `/charity-needs/{charityNeedId}` | Charity | Update charity need | Multipart |
| 29 | DELETE | `/charity-needs/{charityNeedId}` | Charity | Delete charity need | - |
| 30 | PATCH | `/charity-needs/{charityNeedId}/fulfill` | Charity | Mark need fulfilled | - |
| 31 | GET | `/applications/received` | Charity | Need applications received | Query |
| 32 | PATCH | `/applications/{needApplicationId}/accept` | Charity | Accept need application | - |
| 33 | PATCH | `/applications/{needApplicationId}/reject` | Charity | Reject need application | - |
| 34 | POST | `/offers/{offerId}/apply` | Charity | Apply to donor offer | - |
| 35 | GET | `/applications/sent` | Charity | My offer applications | Query |
| 36 | DELETE | `/applications/{offerApplicationId}` | Charity | Cancel offer application | - |

---

## DONOR ORGANIZATION APIs
**Prefix**: `/api/v1/donor-organization`

| # | Method | Endpoint | Auth | Description | Payload |
|---|--------|----------|------|-------------|---------|
| 37 | GET | `/dashboard` | Donor | Dashboard statistics | - |
| 38 | POST | `/offer` | Donor | Create offer | Multipart |
| 39 | GET | `/offer/my-offers` | Donor | My offers | Query |
| 40 | GET | `/offer/my-offers/{offerId}` | Donor | Offer details | - |
| 41 | PUT | `/offer/{offerId}` | Donor | Update offer | Multipart |
| 42 | DELETE | `/offer/{offerId}` | Donor | Delete offer | - |
| 43 | PATCH | `/offer/{offerId}/fulfill` | Donor | Mark offer fulfilled | - |
| 44 | GET | `/offer-applications/received` | Donor | Offer applications received | Query |
| 45 | PATCH | `/offer-applications/{offerApplicationId}/accept` | Donor | Accept offer application | - |
| 46 | PATCH | `/offer-applications/{offerApplicationId}/reject` | Donor | Reject offer application | - |
| 47 | POST | `/charity-needs/{charityNeedId}/apply` | Donor | Apply to charity need | - |
| 48 | GET | `/need-applications/sent` | Donor | My need applications | Query |

---

## PROFILE APIs
**Prefix**: `/api/v1/profile`

| # | Method | Endpoint | Auth | Description | Payload |
|---|--------|----------|------|-------------|---------|
| 49 | GET | `/` | Yes | Get profile | - |
| 50 | PUT | `/` | Yes | Update profile | JSON |
| 51 | PATCH | `/password` | Yes | Change password | JSON |
| 52 | PATCH | `/image` | Yes | Update profile image | Multipart |