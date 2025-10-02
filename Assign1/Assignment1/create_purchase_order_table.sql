-- Create PurchaseOrder table manually
CREATE TABLE IF NOT EXISTS "PurchaseOrder" (
    "Id" uuid NOT NULL,
    "DealerId" uuid NOT NULL,
    "ProductId" uuid NOT NULL,
    "RequestedById" uuid NOT NULL,
    "ApprovedById" uuid NULL,
    "OrderNumber" character varying(50) NOT NULL,
    "RequestedQuantity" integer NOT NULL,
    "UnitPrice" decimal(18,2) NOT NULL,
    "TotalAmount" decimal(18,2) NOT NULL,
    "Status" integer NOT NULL,
    "RequestedDate" timestamp with time zone NOT NULL,
    "ApprovedDate" timestamp with time zone NULL,
    "ExpectedDeliveryDate" timestamp with time zone NULL,
    "ActualDeliveryDate" timestamp with time zone NULL,
    "Reason" character varying(500) NOT NULL,
    "Notes" character varying(1000) NULL,
    "RejectReason" character varying(500) NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_PurchaseOrder" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_PurchaseOrder_Dealer_DealerId" FOREIGN KEY ("DealerId") REFERENCES "Dealer" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_PurchaseOrder_Product_ProductId" FOREIGN KEY ("ProductId") REFERENCES "Product" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_PurchaseOrder_Users_RequestedById" FOREIGN KEY ("RequestedById") REFERENCES "Users" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_PurchaseOrder_Users_ApprovedById" FOREIGN KEY ("ApprovedById") REFERENCES "Users" ("Id") ON DELETE RESTRICT
);

-- Create indexes
CREATE INDEX IF NOT EXISTS "IX_PurchaseOrder_DealerId" ON "PurchaseOrder" ("DealerId");
CREATE INDEX IF NOT EXISTS "IX_PurchaseOrder_ProductId" ON "PurchaseOrder" ("ProductId");
CREATE INDEX IF NOT EXISTS "IX_PurchaseOrder_RequestedById" ON "PurchaseOrder" ("RequestedById");
CREATE INDEX IF NOT EXISTS "IX_PurchaseOrder_ApprovedById" ON "PurchaseOrder" ("ApprovedById");

-- Update __EFMigrationsHistory to mark migration as applied
INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20241002210000_AddPurchaseOrderEntity', '9.0.5')
ON CONFLICT ("MigrationId") DO NOTHING;
