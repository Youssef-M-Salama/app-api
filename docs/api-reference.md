# API Reference
> Charity-Donor Platform — V1

---

## AUTHENTICATION APIs

| # | Method | Endpoint | Auth | Description |
|---|--------|----------|------|-------------|
| 1 | POST | `/api/v1/auth/register` | No | Register new user |
| 2 | POST | `/api/v1/auth/login` | No | User login |
| 3 | POST | `/api/v1/auth/refresh` | No | Refresh access token |
| 4 | POST | `/api/v1/auth/logout` | Yes | User logout |
| 5 | GET | `/api/v1/auth/verify-email` | No | Verify email with token |
| 6 | POST | `/api/v1/auth/resend-verification` | No | Resend verification email |

---

## PUBLIC APIs (Guest Access)

| # | Method | Endpoint | Auth | Description |
|---|--------|----------|------|-------------|
| 7 | GET | `/api/v1/public/statistics` | No | Platform statistics |
| 8 | GET | `/api/v1/public/charity-needs` | No | Browse approved charity needs |
| 9 | GET | `/api/v1/public/offers` | No | Browse available offers |
| 10 | GET | `/api/v1/public/charity-needs/{charityNeedId}` | No | Charity need details |
| 11 | GET | `/api/v1/public/offers/{offerId}` | No | Offer details |

---

## ADMIN APIs

| # | Method | Endpoint | Auth | Description |
|---|--------|----------|------|-------------|
| 12 | GET | `/api/v1/admin/dashboard` | Admin | Dashboard statistics |
| 13 | GET | `/api/v1/admin/verifications/pending` | Admin | Pending verifications |
| 14 | PATCH | `/api/v1/admin/verifications/verify` | Admin | Verify user account |
| 15 | GET | `/api/v1/admin/charity-needs/pending` | Admin | Pending charity needs |
| 16 | PATCH | `/api/v1/admin/charity-needs/approve` | Admin | Approve charity need |
| 17 | PATCH | `/api/v1/admin/charity-needs/reject` | Admin | Reject charity need |
| 18 | GET | `/api/v1/admin/offers/pending` | Admin | Pending donor offers |
| 19 | PATCH | `/api/v1/admin/offers/approve` | Admin | Approve donor offer |
| 20 | PATCH | `/api/v1/admin/offers/reject` | Admin | Reject donor offer |
| 21 | GET | `/api/v1/admin/users` | Admin | All users with filters |
| 22 | PATCH | `/api/v1/admin/users/deactivate` | Admin | Deactivate user account |
| 23 | PATCH | `/api/v1/admin/users/activate` | Admin | Activate user account |

---

## CHARITY APIs

| # | Method | Endpoint | Auth | Description |
|---|--------|----------|------|-------------|
| 24 | GET | `/api/v1/charity/dashboard` | Charity | Dashboard statistics |
| 25 | POST | `/api/v1/charity/charity-needs` | Charity | Create charity need (Multipart) |
| 26 | GET | `/api/v1/charity/charity-needs` | Charity | My charity needs |
| 27 | GET | `/api/v1/charity/charity-needs/{charityNeedId}` | Charity | Charity need details |
| 28 | PUT | `/api/v1/charity/charity-needs/{charityNeedId}` | Charity | Update charity need (Multipart) |
| 29 | DELETE | `/api/v1/charity/charity-needs/{charityNeedId}` | Charity | Delete charity need |
| 30 | PATCH | `/api/v1/charity/charity-needs/{charityNeedId}/fulfill` | Charity | Mark need fulfilled |
| 31 | GET | `/api/v1/charity/applications/received` | Charity | Need applications received |
| 32 | PATCH | `/api/v1/charity/applications/{needApplicationId}/accept` | Charity | Accept need application |
| 33 | PATCH | `/api/v1/charity/applications/{needApplicationId}/reject` | Charity | Reject need application |
| 34 | POST | `/api/v1/charity/offers/{offerId}/apply` | Charity | Apply to donor offer |
| 35 | GET | `/api/v1/charity/applications/sent` | Charity | My offer applications |
| 36 | DELETE | `/api/v1/charity/applications/{offerApplicationId}` | Charity | Cancel offer application |

---

## DONOR ORGANIZATION APIs

| # | Method | Endpoint | Auth | Description |
|---|--------|----------|------|-------------|
| 37 | GET | `/api/v1/donor-organization/dashboard` | Donor | Dashboard statistics |
| 38 | POST | `/api/v1/donor-organization/offer` | Donor | Create offer (Multipart) |
| 39 | GET | `/api/v1/donor-organization/offer/my-offers` | Donor | My offers |
| 40 | GET | `/api/v1/donor-organization/offer/my-offers/{offerId}` | Donor | Offer details |
| 41 | PUT | `/api/v1/donor-organization/offer/{offerId}` | Donor | Update offer (Multipart) |
| 42 | DELETE | `/api/v1/donor-organization/offer/{offerId}` | Donor | Delete offer |
| 43 | PATCH | `/api/v1/donor-organization/offer/{offerId}/fulfill` | Donor | Mark offer fulfilled |
| 44 | GET | `/api/v1/donor-organization/offer-applications/received` | Donor | Offer applications received |
| 45 | PATCH | `/api/v1/donor-organization/offer-applications/{offerApplicationId}/accept` | Donor | Accept offer application |
| 46 | PATCH | `/api/v1/donor-organization/offer-applications/{offerApplicationId}/reject` | Donor | Reject offer application |
| 47 | POST | `/api/v1/donor-organization/charity-needs/{charityNeedId}/apply` | Donor | Apply to charity need |
| 48 | GET | `/api/v1/donor-organization/need-applications/sent` | Donor | My need applications |

---

## PROFILE APIs (All Authenticated Users)

| # | Method | Endpoint | Auth | Description |
|---|--------|----------|------|-------------|
| 49 | GET | `/api/v1/profile` | Yes | Get profile |
| 50 | PUT | `/api/v1/profile` | Yes | Update profile |
| 51 | PATCH | `/api/v1/profile/password` | Yes | Change password |
| 52 | PATCH | `/api/v1/profile/image` | Yes | Update profile image (Multipart) |