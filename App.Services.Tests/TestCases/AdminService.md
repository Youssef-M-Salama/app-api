# AdminServiceTests — Test Coverage

## GetDashboardStatisticsAsync

| Test Case | Expected Outcome |
|-----------|-----------------|
| Repository returns valid stats | `200 OK` with correct values for `PendingVerifications`, `PendingCharityNeeds`, `PendingOffers`, `TotalUsers`, `ActiveCharityNeeds`, `ActiveOffers` |
| Repository throws an exception | `500 Internal Server Error` with message "An unexpected error occurred" |

---

## GetPendingVerificationsAsync

| Test Case | Expected Outcome |
|-----------|-----------------|
| Repository returns pending charities and donors | `200 OK` with both lists populated and correct names |
| Repository throws an exception | `500 Internal Server Error` |

---

## VerifyUserAsync

| Test Case | Expected Outcome |
|-----------|-----------------|
| User found and verified | `200 OK`; `SendAccountVerifiedAsync` email called once |
| User not found (repository returns `false`) | `404 Not Found` |

---

## RejectUserAsync

| Test Case | Expected Outcome |
|-----------|-----------------|
| User found and rejected | `200 OK`; `SendAccountRejectedAsync` email called once |
| User not found | `404 Not Found` |

---

## GetPendingCharityNeedsAsync

| Test Case | Expected Outcome |
|-----------|-----------------|
| Repository returns one pending need, page 1 / size 10 | `200 OK` with one item and `TotalCount = 1` in pagination |

---

## ApproveCharityNeedAsync

| Test Case | Expected Outcome |
|-----------|-----------------|
| Charity need found and approved | `200 OK`; `SendCharityNeedApprovedAsync` email called once |
| Charity need not found | `404 Not Found` |

---

## RejectCharityNeedAsync

| Test Case | Expected Outcome |
|-----------|-----------------|
| Charity need found and rejected | `200 OK`; `SendCharityNeedRejectedAsync` email called once |
| Charity need not found | `404 Not Found` |

---

## GetAllUsersAsync

| Test Case | Expected Outcome |
|-----------|-----------------|
| Repository returns one user with no filters | `200 OK` with one item and correct email |

---

## DeactivateUserAsync

| Test Case | Expected Outcome |
|-----------|-----------------|
| User found | `200 OK` |
| User not found | `404 Not Found` |

---

## ActivateUserAsync

| Test Case | Expected Outcome |
|-----------|-----------------|
| User found | `200 OK` |
| User not found | `404 Not Found` |

---

## GetPendingOffersAsync

| Test Case | Expected Outcome |
|-----------|-----------------|
| Repository returns one pending offer, page 1 / size 10 | `200 OK` with one item and `TotalCount = 1` in pagination |
| Page is 0 | `400 Bad Request` |
| PageSize is 0 | `400 Bad Request` |
| PageSize exceeds 50 | `400 Bad Request` |

---

## ApproveOfferAsync

| Test Case | Expected Outcome |
|-----------|-----------------|
| Offer found and approved | `200 OK`; `SendOfferApprovedAsync` email called once |
| Offer not found | `404 Not Found` |

---

## RejectOfferAsync

| Test Case | Expected Outcome |
|-----------|-----------------|
| Offer found and rejected | `200 OK`; `SendOfferRejectedAsync` email called once |
| Offer not found | `404 Not Found` |