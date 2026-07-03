# AccountServiceTests — Test Coverage

## RegisterAsync

| Test Case | Expected Outcome |
|-----------|-----------------|
| Valid registration data | `201 Created` with success message containing "verify your account" |
| Valid registration but email sending fails | `201 Created` with warning message containing "resend verification" |
| Identity `CreateAsync` fails (e.g. duplicate email error) | `400 Bad Request` with error details |
| Username already taken | `400 Bad Request` |
| Email already taken | `400 Bad Request` |

---

## LoginAsync

| Test Case | Expected Outcome |
|-----------|-----------------|
| Valid credentials, active user, confirmed email | `200 OK` with JWT token, refresh token, and correct role |
| User not found (by username or email) | `401 Unauthorized` |
| Email not confirmed (`SignInResult.NotAllowed`) | `403 Forbidden` |
| User account is inactive | `403 Forbidden` |

---

## VerifyEmailAsync

| Test Case | Expected Outcome |
|-----------|-----------------|
| Valid Base64-encoded token, unverified user | `200 OK` with message containing "verified successfully" |
| Valid Base64 token but Identity `ConfirmEmailAsync` fails | `400 Bad Request` |
| Token is not valid Base64 | `400 Bad Request` |
| Email already confirmed | `200 OK` with message containing "already verified" |
| User not found | `404 Not Found` |

---

## ResendVerificationEmailAsync

| Test Case | Expected Outcome |
|-----------|-----------------|
| Valid unverified user, email sends successfully | `200 OK` with message containing "check your inbox" |
| User not found (anti-enumeration: no error exposed) | `200 OK` |
| User email is already verified | `200 OK` with message containing "already verified" |

---

## RefreshTokenAsync

| Test Case | Expected Outcome |
|-----------|-----------------|
| Valid refresh token, not expired | `200 OK` with new access token and rotated refresh token; `UpdateAsync` called at least once |
| Refresh token not found in database | `401 Unauthorized` |
| Refresh token is expired | `401 Unauthorized`; `UpdateAsync` called with `RefreshToken = null` to revoke |

---

## LogoutAsync

| Test Case | Expected Outcome |
|-----------|-----------------|
| Valid user found | `200 OK`; `UpdateAsync` called with `RefreshToken = null` and `RefreshTokenExpiration = null` |
| User not found | `404 Not Found` |