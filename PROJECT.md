# 🚀 Dapper to Entity Framework Migration Plan

## 📋 Overview

Current state:
- SQLite database with 9 tables
- Dapper for data access
- Command pattern for mutations
- Service layer for business logic

## 🎯 Migration Goals

1. Replace Dapper with Entity Framework Core
2. Maintain existing functionality
3. Keep tests passing throughout migration
4. Single change per commit
5. One iteration per dev branch push

## 🗺️ Iterations

### 1️⃣ Setup (Iteration 1)
- Install required packages:
  - Microsoft.EntityFrameworkCore
  - Microsoft.EntityFrameworkCore.Sqlite
  - Microsoft.EntityFrameworkCore.Design
- Create initial DbContext
- Test: App builds and runs with both Dapper and EF present

### 2️⃣ Entity Generation (Iteration 2)
- Create the EF Models
- Configure entity relationships:
  - Contact -> Activity
  - Contact <-> Tag (many-to-many)
  - Contact <-> Sequence (many-to-many)
  - Sequence -> Email
  - Email -> Broadcast
  - Message standalone
- Add navigation properties
- Test: Entity configurations compile

### 3️⃣ Basic Contact Operations (Iteration 3)
- Migrate basic Contact CRUD operations
- Update ContactSignupCommand
- Add EF-specific contact repository
- Keep Dapper implementation as fallback
- Test: Basic contact operations work

### 4️⃣ Contact Actions (Iteration 4)
- Migrate contact action commands:
  - ContactOptInCommand
  - ContactOptOutCommand
  - ContactDeleteCommand
- Update activity tracking
- Test: Contact action commands work

### 5️⃣ Tag System (Iteration 5)
- Migrate tag operations
- Update ContactTagCommand
- Implement tag queries using EF
- Test: Tag system fully functional

### 6️⃣ Email System (Iteration 6)
- Migrate PostOffice.cs to EF
- Update broadcast handling
- Update message tracking
- Test: Email system works end-to-end

### 7️⃣ Data Migration (Iteration 7)
- Verify data consistency
- Create data migration scripts
- Test migration rollback procedures
- Document migration steps
- Test: Data migration successful

### 8️⃣ Cleanup (Final Iteration)
- Remove Dapper package
- Remove old DB.cs
- Clean up connection handling
- Update dependency injection
- Remove legacy interfaces
- Final test suite run
- Performance testing
- Test: Full system verification

## 🔍 Testing Strategy

Each iteration requires:
- Unit tests passing
- Integration tests passing
- Manual verification
- Performance comparison

## 🚧 Rollback Plan

Per-iteration rollback:
1. Git revert specific commit
2. Run full test suite
3. Verify system operation
4. Push revert to dev

## 📝 Technical Notes

- Keep existing database schema
- Maintain transaction boundaries
- Log query performance metrics
- Document any EF-specific configurations
