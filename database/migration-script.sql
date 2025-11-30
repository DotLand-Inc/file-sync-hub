-- FileSyncHub Migration Script
-- Generated from: InitialCreate (20241129000000)
-- PostgreSQL 16+

-- ===================================================
-- Tables Creation
-- ===================================================

-- Documents Table
CREATE TABLE IF NOT EXISTS "Documents" (
    "Id" uuid NOT NULL,
    "OrganizationId" character varying(100) NOT NULL,
    "Title" character varying(500) NOT NULL,
    "Description" character varying(2000),
    "FileName" character varying(500) NOT NULL,
    "ContentType" character varying(200) NOT NULL,
    "FileSize" bigint NOT NULL,
    "Category" integer NOT NULL,
    "Status" integer NOT NULL,
    "CurrentVersion" integer NOT NULL,
    "S3Key" character varying(1000),
    "WorkflowInstanceId" character varying(100),
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    "CreatedBy" character varying(200),
    "UpdatedBy" character varying(200),
    CONSTRAINT "PK_Documents" PRIMARY KEY ("Id")
);

-- Organization Versioning Configurations Table
CREATE TABLE IF NOT EXISTS "OrganizationVersioningConfigurations" (
    "Id" uuid NOT NULL,
    "OrganizationId" character varying(100) NOT NULL,
    "DefaultVersioningEnabled" boolean NOT NULL,
    "DefaultMaxVersions" integer NOT NULL,
    "IsActive" boolean NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    "CreatedBy" character varying(200),
    "UpdatedBy" character varying(200),
    CONSTRAINT "PK_OrganizationVersioningConfigurations" PRIMARY KEY ("Id")
);

-- Document Status History Table
CREATE TABLE IF NOT EXISTS "DocumentStatusHistory" (
    "Id" uuid NOT NULL,
    "DocumentId" uuid NOT NULL,
    "Status" integer NOT NULL,
    "Comment" character varying(1000) NOT NULL,
    "ChangedBy" character varying(200),
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    "CreatedBy" character varying(200),
    "UpdatedBy" character varying(200),
    CONSTRAINT "PK_DocumentStatusHistory" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_DocumentStatusHistory_Documents_DocumentId"
        FOREIGN KEY ("DocumentId")
        REFERENCES "Documents" ("Id")
        ON DELETE CASCADE
);

-- Document Versions Table
CREATE TABLE IF NOT EXISTS "DocumentVersions" (
    "Id" uuid NOT NULL,
    "DocumentId" uuid NOT NULL,
    "VersionNumber" integer NOT NULL,
    "S3Key" character varying(1000) NOT NULL,
    "FileName" character varying(500) NOT NULL,
    "ContentType" character varying(200) NOT NULL,
    "FileSize" bigint NOT NULL,
    "Comment" character varying(1000),
    "IsActive" boolean NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    "CreatedBy" character varying(200),
    "UpdatedBy" character varying(200),
    CONSTRAINT "PK_DocumentVersions" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_DocumentVersions_Documents_DocumentId"
        FOREIGN KEY ("DocumentId")
        REFERENCES "Documents" ("Id")
        ON DELETE CASCADE
);

-- Category Versioning Configurations Table
CREATE TABLE IF NOT EXISTS "CategoryVersioningConfigurations" (
    "Id" uuid NOT NULL,
    "OrganizationVersioningConfigurationId" uuid NOT NULL,
    "Category" integer NOT NULL,
    "VersioningEnabled" boolean NOT NULL,
    "MaxVersions" integer NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone,
    "CreatedBy" character varying(200),
    "UpdatedBy" character varying(200),
    CONSTRAINT "PK_CategoryVersioningConfigurations" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_CategoryVersioningConfigurations_OrganizationVersioningConf~"
        FOREIGN KEY ("OrganizationVersioningConfigurationId")
        REFERENCES "OrganizationVersioningConfigurations" ("Id")
        ON DELETE CASCADE
);

-- ===================================================
-- Indexes Creation
-- ===================================================

-- Documents Indexes
CREATE INDEX IF NOT EXISTS "IX_Documents_Category" ON "Documents" ("Category");
CREATE INDEX IF NOT EXISTS "IX_Documents_OrganizationId" ON "Documents" ("OrganizationId");
CREATE INDEX IF NOT EXISTS "IX_Documents_Status" ON "Documents" ("Status");
CREATE INDEX IF NOT EXISTS "IX_Documents_WorkflowInstanceId" ON "Documents" ("WorkflowInstanceId");
CREATE INDEX IF NOT EXISTS "IX_Documents_OrganizationId_Category" ON "Documents" ("OrganizationId", "Category");

-- Document Status History Indexes
CREATE INDEX IF NOT EXISTS "IX_DocumentStatusHistory_DocumentId" ON "DocumentStatusHistory" ("DocumentId");
CREATE INDEX IF NOT EXISTS "IX_DocumentStatusHistory_DocumentId_CreatedAt" ON "DocumentStatusHistory" ("DocumentId", "CreatedAt");

-- Document Versions Indexes
CREATE INDEX IF NOT EXISTS "IX_DocumentVersions_DocumentId" ON "DocumentVersions" ("DocumentId");
CREATE UNIQUE INDEX IF NOT EXISTS "IX_DocumentVersions_DocumentId_VersionNumber" ON "DocumentVersions" ("DocumentId", "VersionNumber");

-- Organization Versioning Configurations Indexes
CREATE INDEX IF NOT EXISTS "IX_OrganizationVersioningConfigurations_IsActive" ON "OrganizationVersioningConfigurations" ("IsActive");
CREATE UNIQUE INDEX IF NOT EXISTS "IX_OrganizationVersioningConfigurations_OrganizationId" ON "OrganizationVersioningConfigurations" ("OrganizationId");

-- Category Versioning Configurations Indexes
CREATE UNIQUE INDEX IF NOT EXISTS "IX_CategoryVersioningConfigurations_OrganizationVersioningConf~" ON "CategoryVersioningConfigurations" ("OrganizationVersioningConfigurationId", "Category");

-- ===================================================
-- Comments
-- ===================================================

COMMENT ON TABLE "Documents" IS 'Main documents table storing document metadata';
COMMENT ON TABLE "DocumentVersions" IS 'Document versions with S3 storage keys';
COMMENT ON TABLE "DocumentStatusHistory" IS 'Audit trail of document status changes';
COMMENT ON TABLE "OrganizationVersioningConfigurations" IS 'Organization-level versioning settings';
COMMENT ON TABLE "CategoryVersioningConfigurations" IS 'Category-specific versioning overrides';

-- ===================================================
-- Summary
-- ===================================================

SELECT
    'Tables created: ' || COUNT(*) AS summary
FROM information_schema.tables
WHERE table_schema = 'public'
    AND table_name IN (
        'Documents',
        'DocumentVersions',
        'DocumentStatusHistory',
        'OrganizationVersioningConfigurations',
        'CategoryVersioningConfigurations'
    );
