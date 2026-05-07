# User Stories
> Charity-Donor Platform — V1

---

## GUEST USER

| ID | User Story | Acceptance Criteria |
|----|------------|---------------------|
| G1 | View platform statistics | See total donations, charities, donors, active charity needs, and active offers |
| G2 | Browse charity needs | See all approved charity needs with filters (category, city, governorate) and pagination |
| G3 | Browse donor offers | See all available offers with filters (category, city, governorate) and pagination |
| G4 | View charity need details | See full details of any approved charity need including product image |
| G5 | View offer details | See full details of any available offer including product image |
| G6 | Register as charity | Create account with charity details and location |
| G7 | Register as donor organization | Create account with organization details and location |

---

## ADMIN USER

| ID | User Story | Acceptance Criteria |
|----|------------|---------------------|
| A1 | View dashboard statistics | See pending verifications, pending charity needs, total users, and pending offers |
| A2 | Review pending verifications | See list of unverified charities and donor organizations |
| A3 | Verify accounts | Approve user registration, triggering automatic email notifications |
| A4 | Review pending charity needs | See list of charity needs waiting for approval with all details |
| A5 | Approve/Reject charity needs | Change status of charity needs, making them visible to donors if approved |
| A6 | Review pending offers | See list of offers waiting for approval |
| A7 | Approve/Reject offers | Change status of offers, making them visible to charities if approved |
| A8 | Manage users | View all users with role/status filters; activate or deactivate accounts |

---

## CHARITY USER

| ID | User Story | Acceptance Criteria |
|----|------------|---------------------|
| C1 | View dashboard | See stats for charity needs, received applications, and sent applications |
| C2 | Create charity need | Post need with product, quantity (decimal), unit, category, priority, and product image |
| C3 | Manage charity needs | View, update, or delete my own needs (updates/deletes restricted to Pending status) |
| C4 | Fulfill charity need | Mark an approved need as fulfilled to conclude the lifecycle |
| C5 | Review received applications | See donors who applied to my needs; view their organization profiles |
| C6 | Accept/Reject applications | Handle incoming donor applications for my needs with state guards |
| C7 | Apply to donor offers | Browse approved offers and submit applications to donors |
| C8 | Track sent applications | See all offers I applied to with status tracking (Accepted/Rejected) |
| C9 | Manage profile | Update organization info, location, contact details, profile image, and password |

---

## DONOR ORGANIZATION USER

| ID | User Story | Acceptance Criteria |
|----|------------|---------------------|
| D1 | View dashboard | See stats for offers, received applications, and sent applications |
| D2 | Create offer | Post offer with product, quantity, unit, category, expiry date, and product image |
| D3 | Manage offers | View, update, or delete my own offers (restricted to Pending status) |
| D4 | Fulfill offer | Mark an approved offer as fulfilled once donation is delivered |
| D5 | Review received applications | See charities who applied to my offers; view their charity profiles |
| D6 | Accept/Reject applications | Handle incoming charity applications for my offers |
| D7 | Apply to charity needs | Browse approved needs and submit applications to charities |
| D8 | Track sent applications | See all charity needs I applied to with status tracking |
| D9 | Manage profile | Update organization info, location, contact details, profile image, and password |