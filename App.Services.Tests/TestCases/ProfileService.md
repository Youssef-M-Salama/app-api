# ProfileServiceTests — Test Coverage

## GetProfileAsync

| Test Case | Expected Outcome |
|-----------|-----------------|
| User is a Charity role | `200 OK`; `CharityDetails` populated, `DonorDetails` is null, `IsVerified` reflects charity's status |
| User is a DonorOrganization role | `200 OK`; `DonorDetails` populated, `CharityDetails` is null, `IsVerified` reflects donor's status |
| User not found | `404 Not Found` |
| Repository/manager throws an exception | `500 Internal Server Error` |

---

## UpdateProfileAsync

| Test Case | Expected Outcome |
|-----------|-----------------|
| Valid update request | `200 OK`; `UpdateAsync` called with new `PhoneNumber` and `City` values |
| User not found | `404 Not Found` |
| `UpdateAsync` fails (Identity errors returned) | `400 Bad Request` with error details |
| Manager throws an exception | `500 Internal Server Error` |

---

## ChangePasswordAsync

| Test Case | Expected Outcome |
|-----------|-----------------|
| Valid current password and matching new password | `200 OK` |
| User not found | `404 Not Found` |
| Current password is wrong (`ChangePasswordAsync` fails) | `400 Bad Request` with error details |
| Manager throws an exception | `500 Internal Server Error` |

---

## UpdateProfileImageAsync

| Test Case | Expected Outcome |
|-----------|-----------------|
| Valid image file | `200 OK`; old image deleted via `DeleteImageAsync`; `SaveImageAsync` called with `ImageFolder.Users` |
| User not found | `404 Not Found` |
| `SaveImageAsync` returns bad request (invalid format/size) | `400 Bad Request` |
| `UpdateAsync` fails after saving new image | `400 Bad Request`; new image rolled back by calling `DeleteImageAsync` on the new path |
| Manager throws an exception | `500 Internal Server Error` |