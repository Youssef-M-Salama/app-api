# PublicServiceTests — Test Coverage

## GetApprovedCharityNeedsAsync

| Test Case | Expected Outcome |
|-----------|-----------------|
| Valid page and page size (page 2, size 5) | `200 OK` with 5 items; correct `Page`, `TotalCount`, `TotalPages`, `HasNext`, `HasPrevious` in pagination |
| Page is 0 or negative | `400 Bad Request` |
| PageSize is 0 or negative | `400 Bad Request` |
| PageSize exceeds limit of 50 | `400 Bad Request` |
| Repository throws an exception | `500 Internal Server Error` |

---

## GetApprovedCharityNeedByIdAsync

| Test Case | Expected Outcome |
|-----------|-----------------|
| Charity need found by ID | `200 OK` with correct `ProductName` and `CharityNeedId` |
| Charity need not found (repository returns null) | `404 Not Found` |
| Repository throws an exception | `500 Internal Server Error` |

---

## GetApprovedOffersAsync

| Test Case | Expected Outcome |
|-----------|-----------------|
| Valid page and page size (page 2, size 5) | `200 OK` with 5 items; correct `Page`, `TotalCount`, `TotalPages`, `HasNext`, `HasPrevious` in pagination |
| Page is 0 or negative | `400 Bad Request` |
| PageSize is 0 or negative | `400 Bad Request` |
| PageSize exceeds limit of 50 | `400 Bad Request` |
| Repository throws an exception | `500 Internal Server Error` |

---

## GetApprovedOfferByIdAsync

| Test Case | Expected Outcome |
|-----------|-----------------|
| Offer found by ID | `200 OK` with correct `ProductName` and `OfferId` |
| Offer not found (repository returns null) | `404 Not Found` |
| Repository throws an exception | `500 Internal Server Error` |

---

## GetStatisticsAsync

| Test Case | Expected Outcome |
|-----------|-----------------|
| All repositories return valid counts | `200 OK`; `TotalDonations` = fulfilled needs + fulfilled offers; `TotalItemsDonated` = sum of quantities from both; `TotalCharities`, `TotalDonors`, `ActiveCharityNeeds`, `ActiveOffers` all correct |
| Any repository throws an exception | `500 Internal Server Error` |