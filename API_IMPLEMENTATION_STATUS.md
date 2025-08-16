# UniFi Access API Implementation Status

> Tracking implementation progress for UniFi Access API v3.3.21

## 📊 Overall Progress

- **Total Endpoints**: 120+
- **Implemented**: 30/120 (25.0%)
- **Tested**: 0/120 (0%)
- **Documentation**: 30/120 (25.0%)

### Priority Breakdown
- **P0 (Core)**: 100% (19/19) - ✅ Users (7), ✅ Access Policies (5), ✅ Doors (7)
- **P1 (Essential)**: 18.3% (11/60) - Credentials (5: 4 NFC, 1 PIN), User credential assignments (4), Devices (1)  
- **P2 (Extended)**: 0% - Visitors, Schedules, Holiday Groups
- **P3 (Advanced)**: 0% - WebSockets, Webhooks, Identity, Certificates

---

## 🏗️ Infrastructure Components

### Base Components
- [x] `UnifiAccessClient` - Main client class
- [x] `IUnifiAccessClient` - Client interface
- [x] `UnifiAccessJsonContext` - JSON source generator context
- [x] `ServiceCollectionExtensions` - DI registration
- [x] Authentication handler
- [x] Error response handling
- [x] Retry policies with Polly
- [ ] Rate limiting support

---

## 📁 Section 3: Users (26 endpoints)

### User Management **[P0]**
- [x] `POST /api/v1/users` - User Registration (3.2)
- [x] `PUT /api/v1/users/{id}` - Update User (3.3)
- [x] `GET /api/v1/users/{id}` - Fetch User (3.4)
- [x] `GET /api/v1/users` - Fetch All Users (3.5)
- [x] `DELETE /api/v1/users/{id}` - Delete User (3.23)
- [x] `GET /api/v1/users/search` - Search Users (3.24)
- [x] `POST /api/v1/users/{id}/photo` - Upload User Profile Picture (3.30)

### User Access Policies **[P0]**
- [x] `POST /api/v1/users/{id}/access_policies` - Assign Access Policy to User (3.6)
- [x] `GET /api/v1/users/{id}/access_policies` - Fetch Access Policies Assigned to User (3.20)

### User Credentials **[P1]**
- [x] `POST /api/v1/users/{id}/nfc_cards` - Assign NFC Card to User (3.7)
- [x] `DELETE /api/v1/users/{id}/nfc_cards/{card_id}` - Unassign NFC Card from User (3.8)
- [x] `POST /api/v1/users/{id}/pin_codes` - Assign PIN Code to User (3.9)
- [x] `DELETE /api/v1/users/{id}/pin_codes` - Unassign PIN Code from User (3.10)
- [ ] `POST /api/v1/users/{id}/touch_passes` - Assign Touch Pass to User (3.25)
- [ ] `DELETE /api/v1/users/{id}/touch_passes/{pass_id}` - Unassign Touch Pass from User (3.26)
- [ ] `POST /api/v1/users/touch_passes/batch` - Batch Assign Touch Passes to Users (3.27)
- [ ] `POST /api/v1/users/{id}/license_plates` - Assign License Plate Numbers to User (3.28)
- [ ] `DELETE /api/v1/users/{id}/license_plates` - Unassign License Plate Numbers from User (3.29)

### User Groups **[P1]**
- [ ] `POST /api/v1/user_groups` - Create User Group (3.11)
- [ ] `GET /api/v1/user_groups` - Fetch All User Groups (3.12)
- [ ] `GET /api/v1/user_groups/{id}` - Fetch User Group (3.13)
- [ ] `PUT /api/v1/user_groups/{id}` - Update User Group (3.14)
- [ ] `DELETE /api/v1/user_groups/{id}` - Delete User Group (3.15)
- [ ] `POST /api/v1/user_groups/{id}/users` - Assign User to User Group (3.16)
- [ ] `DELETE /api/v1/user_groups/{id}/users/{user_id}` - Unassign User from User Group (3.17)
- [ ] `GET /api/v1/user_groups/{id}/users` - Fetch Users in User Group (3.18, 3.19)
- [ ] `POST /api/v1/user_groups/{id}/access_policies` - Assign Access Policy to User Group (3.21)
- [ ] `GET /api/v1/user_groups/{id}/access_policies` - Fetch Access Policies Assigned to User Group (3.22)

**Service Interface**: `IUserService`

---

## 📁 Section 4: Visitors (13 endpoints)

### Visitor Management **[P2]**
- [ ] `POST /api/v1/visitors` - Create Visitor (4.2)
- [ ] `GET /api/v1/visitors/{id}` - Fetch Visitor (4.3)
- [ ] `GET /api/v1/visitors` - Fetch All Visitors (4.4)
- [ ] `PUT /api/v1/visitors/{id}` - Update Visitor (4.5)
- [ ] `DELETE /api/v1/visitors/{id}` - Delete Visitor (4.6)

### Visitor Credentials **[P2]**
- [ ] `POST /api/v1/visitors/{id}/nfc_cards` - Assign NFC Card to Visitor (4.7)
- [ ] `DELETE /api/v1/visitors/{id}/nfc_cards/{card_id}` - Unassign NFC Card from Visitor (4.8)
- [ ] `POST /api/v1/visitors/{id}/pin_codes` - Assign PIN Code to Visitor (4.9)
- [ ] `DELETE /api/v1/visitors/{id}/pin_codes` - Unassign PIN Code from Visitor (4.10)
- [ ] `POST /api/v1/visitors/{id}/qr_codes` - Assign QR Code to Visitor (4.11)
- [ ] `DELETE /api/v1/visitors/{id}/qr_codes` - Unassign QR Code from Visitor (4.12)
- [ ] `POST /api/v1/visitors/{id}/license_plates` - Assign License Plate to Visitor (4.13)
- [ ] `DELETE /api/v1/visitors/{id}/license_plates` - Unassign License Plate from Visitor (4.14)

**Service Interface**: `IVisitorService`

---

## 📁 Section 5: Access Policies (12 endpoints)

### Access Policy Management **[P0]**
- [x] `POST /api/v1/access_policies` - Create Access Policy (5.2)
- [x] `PUT /api/v1/access_policies/{id}` - Update Access Policy (5.3)
- [x] `DELETE /api/v1/access_policies/{id}` - Delete Access Policy (5.4)
- [x] `GET /api/v1/access_policies/{id}` - Fetch Access Policy (5.5)
- [x] `GET /api/v1/access_policies` - Fetch All Access Policies (5.6)

### Holiday Groups **[P2]**
- [ ] `POST /api/v1/holiday_groups` - Create Holiday Group (5.8)
- [ ] `PUT /api/v1/holiday_groups/{id}` - Update Holiday Group (5.9)
- [ ] `DELETE /api/v1/holiday_groups/{id}` - Delete Holiday Group (5.10)
- [ ] `GET /api/v1/holiday_groups/{id}` - Fetch Holiday Group (5.11)
- [ ] `GET /api/v1/holiday_groups` - Fetch All Holiday Groups (5.12)

### Schedules **[P2]**
- [ ] `POST /api/v1/schedules` - Create Schedule (5.14)
- [ ] `PUT /api/v1/schedules/{id}` - Update Schedule (5.15)
- [ ] `GET /api/v1/schedules/{id}` - Fetch Schedule (5.16)
- [ ] `GET /api/v1/schedules` - Fetch All Schedules (5.17)
- [ ] `DELETE /api/v1/schedules/{id}` - Delete Schedule (5.18)

**Service Interface**: `IAccessPolicyService`

---

## 📁 Section 6: Credentials (19 endpoints)

### PIN Code Management **[P1]**
- [x] `GET /api/v1/credentials/pin_codes/generate` - Generate PIN Code (6.1)

### NFC Card Management **[P1]**
- [x] `POST /api/v1/credentials/nfc_cards/enroll` - Enroll NFC Card (6.2)
- [x] `GET /api/v1/credentials/nfc_cards/enroll/{session_id}` - Fetch NFC Card Enrollment Status (6.3)
- [x] `DELETE /api/v1/credentials/nfc_cards/enroll/{session_id}` - Remove NFC Card Enrollment Session (6.4)
- [ ] `GET /api/v1/credentials/nfc_cards/{id}` - Fetch NFC Card (6.7)
- [ ] `GET /api/v1/credentials/nfc_cards` - Fetch All NFC Cards (6.8)
- [ ] `DELETE /api/v1/credentials/nfc_cards/{id}` - Delete NFC Card (6.9)
- [ ] `PUT /api/v1/credentials/nfc_cards/{id}` - Update NFC Card (6.10)
- [ ] `POST /api/v1/credentials/nfc_cards/import` - Import Third-Party NFC Cards (6.19)

### Touch Pass Management **[P2]**
- [ ] `GET /api/v1/credentials/touch_passes` - Fetch Touch Pass List (6.12)
- [ ] `GET /api/v1/credentials/touch_passes/search` - Search Touch Pass (6.13)
- [ ] `GET /api/v1/credentials/touch_passes/assignable` - Fetch All Assignable Touch Passes (6.14)
- [ ] `PUT /api/v1/credentials/touch_passes/{id}` - Update Touch Pass (6.15)
- [ ] `GET /api/v1/credentials/touch_passes/{id}` - Fetch Touch Pass Details (6.16)
- [ ] `POST /api/v1/credentials/touch_passes/purchase` - Purchase Touch Passes (6.17)
- [ ] `GET /api/v1/credentials/qr_codes/{id}/image` - Download QR Code Image (6.18)

**Service Interface**: `ICredentialService`

---

## 📁 Section 7: Doors (13 endpoints)

### Door Management **[P0]**
- [x] `GET /api/v1/doors/{id}` - Fetch Door (7.7)
- [x] `GET /api/v1/doors` - Fetch All Doors (7.8)
- [x] `POST /api/v1/doors/{id}/unlock` - Remote Door Unlocking (7.9)
- [x] `PUT /api/v1/doors/{id}/lock_rule` - Set Temporary Door Locking Rule (7.10)
- [x] `GET /api/v1/doors/{id}/lock_rule` - Fetch Door Locking Rule (7.11)
- [x] `PUT /api/v1/doors/{id}/emergency` - Set Door Emergency Status (7.12)
- [x] `GET /api/v1/doors/{id}/emergency` - Fetch Door Emergency Status (7.13)

### Door Groups **[P1]**
- [ ] `GET /api/v1/door_groups/topology` - Fetch Door Group Topology (7.1)
- [ ] `POST /api/v1/door_groups` - Create Door Group (7.2)
- [ ] `GET /api/v1/door_groups/{id}` - Fetch Door Group (7.3)
- [ ] `PUT /api/v1/door_groups/{id}` - Update Door Group (7.4)
- [ ] `GET /api/v1/door_groups` - Fetch All Door Groups (7.5)
- [ ] `DELETE /api/v1/door_groups/{id}` - Delete Door Group (7.6)

**Service Interface**: `IDoorService`

---

## 📁 Section 8: Devices (3 endpoints)

### Device Management **[P2]**
- [x] `GET /api/v1/devices` - Fetch Devices (8.1)
- [ ] `GET /api/v1/devices/{id}/access_methods` - Fetch Access Device's Access Method Settings (8.2)
- [ ] `PUT /api/v1/devices/{id}/access_methods` - Update Access Device's Access Method Settings (8.3)

**Service Interface**: `IDeviceService`

---

## 📁 Section 9: System Logs (5 endpoints)

### Audit Logs **[P2]**
- [ ] `GET /api/v1/system_logs` - Fetch System Logs (9.2)
- [ ] `POST /api/v1/system_logs/export` - Export System Logs (9.3)
- [ ] `GET /api/v1/system_logs/resources` - Fetch Resources in System Logs (9.4)
- [ ] `GET /api/v1/system_logs/static_resources` - Fetch Static Resources in System Logs (9.5)

**Service Interface**: `ISystemLogService`

---

## 📁 Section 10: UniFi Identity (6 endpoints)

### Identity Integration **[P3]**
- [ ] `POST /api/v1/identity/invitations` - Send UniFi Identity Invitations (10.1)
- [ ] `GET /api/v1/identity/resources/available` - Fetch Available Resources (10.2)
- [ ] `POST /api/v1/identity/users/{id}/resources` - Assign Resources to Users (10.3)
- [ ] `GET /api/v1/identity/users/{id}/resources` - Fetch Resources Assigned to Users (10.4)
- [ ] `POST /api/v1/identity/user_groups/{id}/resources` - Assign Resources to User Groups (10.5)
- [ ] `GET /api/v1/identity/user_groups/{id}/resources` - Fetch Resources Assigned to User Groups (10.6)

**Service Interface**: `IIdentityService`

---

## 📁 Section 11: Notifications (7 endpoints)

### Real-time Notifications **[P3]**
- [ ] `WS /api/v1/notifications` - Fetch Notifications via WebSocket (11.1)

### Webhooks **[P3]**
- [ ] `GET /api/v1/webhooks/events` - List of Supported Webhook Events (11.2)
- [ ] `GET /api/v1/webhooks/endpoints` - Fetch Webhook Endpoints List (11.3)
- [ ] `POST /api/v1/webhooks/endpoints` - Add Webhook Endpoints (11.4)
- [ ] `PUT /api/v1/webhooks/endpoints/{id}` - Update Webhook Endpoints (11.5)
- [ ] `DELETE /api/v1/webhooks/endpoints/{id}` - Delete Webhook Endpoints (11.6)
- [ ] `POST /api/v1/webhooks/endpoints/{id}/allow` - Allow Webhook Endpoint to Receive Events (11.7)

**Service Interface**: `INotificationService`

---

## 📁 Section 12: HTTPS Certificates (2 endpoints)

### Certificate Management **[P3]**
- [ ] `POST /api/v1/certificates` - Upload HTTPS Certificate (12.1)
- [ ] `DELETE /api/v1/certificates` - Delete HTTPS Certificate (12.2)

**Service Interface**: `ICertificateService`

---

## 🧪 Testing Matrix

### Unit Testing
- [ ] Model serialization/deserialization tests
- [ ] Service method tests with mocked RestClient
- [ ] Error handling tests
- [ ] Retry policy tests

### Integration Testing
- [ ] API endpoint integration tests (requires mock server)
- [ ] Authentication flow tests
- [ ] Rate limiting tests
- [ ] WebSocket connection tests

### AOT Compatibility
- [ ] Zero warnings on `dotnet publish`
- [ ] JSON source generator verification
- [ ] No reflection usage verification

---

## 📝 Documentation Requirements

### For Each Endpoint
- [ ] XML documentation on public methods
- [ ] Request/Response model documentation
- [ ] Example usage in README
- [ ] Error handling documentation

---

## 🎯 Implementation Guidelines

1. **Start with P0 (Core) endpoints** - These are essential for basic functionality
2. **Implement models first** - Create all request/response DTOs
3. **Add to JsonContext** - Register all models with source generator
4. **Create service interface** - Define method signatures
5. **Implement service** - Add actual implementation
6. **Write unit tests** - Test each method
7. **Add integration tests** - Test against mock API
8. **Update this tracking document** - Check off completed items

---

## 📈 Completion Metrics

Last Updated: 2025-08-16

```
P0 Core:       [██████████] 100%
P1 Essential:  [░░░░░░░░░░] 0%
P2 Extended:   [░░░░░░░░░░] 0%
P3 Advanced:   [░░░░░░░░░░] 0%
Overall:       [██░░░░░░░░] 15.8%
```

---

## 🔗 Related Documents

- [API Reference](Unifi.NET.Access/UniFi_Access_API_Reference.md)
- [CLAUDE.md](CLAUDE.md) - Development guidelines
- [README.md](README.md) - Project overview
- [CONTRIBUTING.md](CONTRIBUTING.md) - Contribution guidelines