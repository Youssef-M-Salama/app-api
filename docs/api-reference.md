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
| 15 | PATCH | `/verifications/in-review` | Admin | Mark registration as in-review | JSON |
| 16 | GET | `/charity-needs/pending` | Admin | Pending charity needs | Query |
| 17 | PATCH | `/charity-needs/approve` | Admin | Approve charity need | JSON |
| 18 | PATCH | `/charity-needs/reject` | Admin | Reject charity need | JSON |
| 19 | GET | `/offers/pending` | Admin | Pending donor offers | Query |
| 20 | PATCH | `/offers/approve` | Admin | Approve donor offer | JSON |
| 21 | PATCH | `/offers/reject` | Admin | Reject donor offer | JSON |
| 22 | GET | `/users` | Admin | All users with filters | Query |
| 23 | PATCH | `/users/deactivate` | Admin | Deactivate user account | JSON |
| 24 | PATCH | `/users/activate` | Admin | Activate user account | JSON |

---

## CHARITY APIs
**Prefix**: `/api/v1/charity`

| # | Method | Endpoint | Auth | Description | Payload |
|---|--------|----------|------|-------------|---------|
| 25 | GET | `/dashboard` | Charity | Dashboard statistics | - |
| 26 | POST | `/charity-needs` | Charity | Create charity need | Multipart |
| 27 | GET | `/charity-needs` | Charity | My charity needs | Query |
| 28 | GET | `/charity-needs/{charityNeedId}` | Charity | Charity need details | - |
| 29 | PUT | `/charity-needs/{charityNeedId}` | Charity | Update charity need | Multipart |
| 30 | DELETE | `/charity-needs/{charityNeedId}` | Charity | Delete charity need | - |
| 31 | PATCH | `/charity-needs/{charityNeedId}/fulfill` | Charity | Mark need fulfilled | - |
| 32 | GET | `/applications/received` | Charity | Need applications received | Query |
| 33 | PATCH | `/applications/{needApplicationId}/accept` | Charity | Accept need application | - |
| 34 | PATCH | `/applications/{needApplicationId}/reject` | Charity | Reject need application | - |
| 35 | POST | `/offers/{offerId}/apply` | Charity | Apply to donor offer | - |
| 36 | GET | `/applications/sent` | Charity | My offer applications | Query |
| 37 | DELETE | `/applications/{offerApplicationId}` | Charity | Cancel offer application | - |
| 38 | PUT | `/verification-data` | Charity | Update verification metadata and PDFs | Multipart |

---

## DONOR ORGANIZATION APIs
**Prefix**: `/api/v1/donor-organization`

| # | Method | Endpoint | Auth | Description | Payload |
|---|--------|----------|------|-------------|---------|
| 39 | GET | `/dashboard` | Donor | Dashboard statistics | - |
| 40 | POST | `/offer` | Donor | Create offer | Multipart |
| 41 | GET | `/offer/my-offers` | Donor | My offers | Query |
| 42 | GET | `/offer/my-offers/{offerId}` | Donor | Offer details | - |
| 43 | PUT | `/offer/{offerId}` | Donor | Update offer | Multipart |
| 44 | DELETE | `/offer/{offerId}` | Donor | Delete offer | - |
| 45 | PATCH | `/offer/{offerId}/fulfill` | Donor | Mark offer fulfilled | - |
| 46 | GET | `/offer-applications/received` | Donor | Offer applications received | Query |
| 47 | PATCH | `/offer-applications/{offerApplicationId}/accept` | Donor | Accept offer application | - |
| 48 | PATCH | `/offer-applications/{offerApplicationId}/reject` | Donor | Reject offer application | - |
| 49 | POST | `/charity-needs/{charityNeedId}/apply` | Donor | Apply to charity need | - |
| 50 | GET | `/need-applications/sent` | Donor | My need applications | Query |
| 51 | DELETE | `/need-applications/{needApplicationId}` | Donor | Cancel need application | - |
| 52 | PUT | `/verification-data` | Donor | Update verification metadata and PDFs | Multipart |

---

## PROFILE APIs
**Prefix**: `/api/v1/profile`

| # | Method | Endpoint | Auth | Description | Payload |
|---|--------|----------|------|-------------|---------|
| 53 | GET | `/` | Yes | Get profile | - |
| 54 | PUT | `/` | Yes | Update profile | JSON |
| 55 | PATCH | `/password` | Yes | Change password | JSON |
| 56 | PATCH | `/image` | Yes | Update profile image | Multipart |