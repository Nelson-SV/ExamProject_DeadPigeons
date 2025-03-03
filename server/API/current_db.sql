drop schema public cascade;
create schema public;
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TABLE "AspNetRoles"
(
    "Id"               text NOT NULL,
    "Name"             character varying(256),
    "NormalizedName"   character varying(256),
    "ConcurrencyStamp" text,
    CONSTRAINT "PK_AspNetRoles" PRIMARY KEY ("Id")
);

CREATE TABLE "AspNetUsers"
(
    "Id"                   text    NOT NULL,
    "UserName"             character varying(256),
    "NormalizedUserName"   character varying(256),
    "Email"                character varying(256),
    "NormalizedEmail"      character varying(256),
    "EmailConfirmed"       boolean NOT NULL,
    "PasswordHash"         text,
    "SecurityStamp"        text,
    "ConcurrencyStamp"     text,
    "PhoneNumber"          text,
    "PhoneNumberConfirmed" boolean NOT NULL,
    "TwoFactorEnabled"     boolean NOT NULL,
    "LockoutEnd"           timestamp with time zone,
    "LockoutEnabled"       boolean NOT NULL,
    "AccessFailedCount"    integer NOT NULL,
    CONSTRAINT "PK_AspNetUsers" PRIMARY KEY ("Id")
);


CREATE TABLE "Game"
(
    "GUID"             text                     NOT NULL,
    "StartDate"        timestamp with time zone NOT NULL,
    "ExtractionDate"   timestamp with time zone,
    "ExtractedNumbers" int[],
    "Revenue"          integer,
    "RolloverValue"    integer,
    "Status"           boolean,
    CONSTRAINT "PK_Game" PRIMARY KEY ("GUID")
);



CREATE TABLE "AspNetRoleClaims"
(
    "Id"         integer GENERATED BY DEFAULT AS IDENTITY,
    "RoleId"     text NOT NULL,
    "ClaimType"  text,
    "ClaimValue" text,
    CONSTRAINT "PK_AspNetRoleClaims" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_AspNetRoleClaims_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "AspNetRoles" ("Id") ON DELETE CASCADE
);



CREATE TABLE "AutomatedTickets"
(
    "GUID"         UUID DEFAULT uuid_generate_v4() NOT NULL,
    "PurchaseDate" timestamp with time zone        NOT NULL,
    "Sequence"     integer[]                       NOT NULL,
    "UserId"       text                            NOT NULL,
    "PriceValue"   integer                         NOT NULL,
    "IsActive"     boolean                         NOT NULL,
    CONSTRAINT "PK_AutomatedTickets" PRIMARY KEY ("GUID"),
    CONSTRAINT "FK_AutomatedTickets_AspNetUser_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);


CREATE TABLE "Balance"
(
    "GUID"   UUID default public.uuid_generate_v4() NOT NULL,
    "Value"  integer                                NOT NULL,
    "UserId" text                                   NOT NULL,
    CONSTRAINT "PK_Balance" PRIMARY KEY ("GUID"),
    CONSTRAINT "FK_Balance_AspNetUser_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);


CREATE TABLE "Payments"
(
    "GUID"          text                     NOT NULL,
    name            text                     NOT NULL,
    bucket          text                     NOT NULL,
    "timeCreated"   timestamp with time zone NOT NULL,
    updated         timestamp with time zone NOT NULL,
    "mediaLink"     text                     NOT NULL,
    "UserId"        text                     NOT NULL,
    "transactionId" varchar(250),
    "pending"       boolean,
    "value"         integer                  not null,
    CONSTRAINT "PK_Payments" PRIMARY KEY ("GUID"),
    CONSTRAINT "FK_Payments_AspNetUser_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);


CREATE TABLE "UserProfile"
(
    "UserId"            text NOT NULL,
    "UserName"          text,
    "ProfilePictureUrl" text,
    "IsActive"         boolean,
    CONSTRAINT "PK_UserProfile" PRIMARY KEY ("UserId"),
    CONSTRAINT "FK_UserProfile_AspNetUser_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);


CREATE TABLE "AspNetUserClaims"
(
    "Id"         integer GENERATED BY DEFAULT AS IDENTITY,
    "UserId"     text NOT NULL,
    "ClaimType"  text,
    "ClaimValue" text,
    CONSTRAINT "PK_AspNetUserClaims" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_AspNetUserClaims_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);


CREATE TABLE "AspNetUserLogins"
(
    "LoginProvider"       text NOT NULL,
    "ProviderKey"         text NOT NULL,
    "ProviderDisplayName" text,
    "UserId"              text NOT NULL,
    CONSTRAINT "PK_AspNetUserLogins" PRIMARY KEY ("LoginProvider", "ProviderKey"),
    CONSTRAINT "FK_AspNetUserLogins_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);


CREATE TABLE "AspNetUserRoles"
(
    "UserId" text NOT NULL,
    "RoleId" text NOT NULL,
    CONSTRAINT "PK_AspNetUserRoles" PRIMARY KEY ("UserId", "RoleId"),
    CONSTRAINT "FK_AspNetUserRoles_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "AspNetRoles" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_AspNetUserRoles_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);


CREATE TABLE "AspNetUserTokens"
(
    "UserId"        text NOT NULL,
    "LoginProvider" text NOT NULL,
    "Name"          text NOT NULL,
    "Value"         text,
    CONSTRAINT "PK_AspNetUserTokens" PRIMARY KEY ("UserId", "LoginProvider", "Name"),
    CONSTRAINT "FK_AspNetUserTokens_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);


CREATE TABLE "GameTickets"
(
    "Id"           SERIAL PRIMARY KEY,
    "GUID"         UUID default uuid_generate_v4() NOT NULL,
    "PurchaseDate" timestamp with time zone        NOT NULL,
    "Sequence"     integer[]                       NOT NULL,
    "UserId"       text                            NOT NULL,
    "PriceValue"   integer                         NOT NULL,
    "GameId"       text                            NOT NULL,
    CONSTRAINT "FK_GameTickets_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_GameTickets_Game_GameId" FOREIGN KEY ("GameId") REFERENCES "Game" ("GUID") ON DELETE CASCADE
);

Create TABLE "WinningPlayers"
(
    "UserId" text not null
        constraint "WinningPlayers_AspNetUsers_Id_fk"
            references public."AspNetUsers",
    "GameId" text not null
        constraint "WinningPlayers_Game_GUID_fk"
            references public."Game"
);

CREATE TABLE "WeeklyTicketSummary"
(
    "GameId"         text    NOT NULL,
    "TotalTickets"   integer NOT NULL,
    "WinningTickets" integer NOT NULL,
    "LosingTickets"  integer NOT NULL,
    CONSTRAINT "PK_WeeklyTicketSummary" PRIMARY KEY ("GameId"),
    CONSTRAINT "FK_WeeklyTicketSummary_Game_GameId" FOREIGN KEY ("GameId") REFERENCES "Game" ("GUID") ON DELETE CASCADE
);

CREATE TABLE "TopUpValues"
(
    "TopUpValue" INTEGER NOT NULL UNIQUE,
    CONSTRAINT "PK_TopUpValue" PRIMARY KEY ("TopUpValue")
);

CREATE TABLE "TicketPrices"
(
    "Created"        timestamp with time zone NOT NULL,
    "Updated"        timestamp with time zone,
    "NumberOfFields" integer                  NOT NULL,
    "Price"          integer                  NOT NULL,
    "GUID"           uuid                  NOT NULL,
    CONSTRAINT "ticketprices_pk" PRIMARY KEY ("GUID"),
    CONSTRAINT "UQ_NumberOfFields_Price" UNIQUE ("NumberOfFields", "Price")
);



CREATE INDEX "IX_AspNetRoleClaims_RoleId" ON "AspNetRoleClaims" ("RoleId");


CREATE UNIQUE INDEX "RoleNameIndex" ON "AspNetRoles" ("NormalizedName");



CREATE INDEX "IX_AspNetUserClaims_UserId" ON "AspNetUserClaims" ("UserId");



CREATE INDEX "IX_AspNetUserLogins_UserId" ON "AspNetUserLogins" ("UserId");


CREATE INDEX "IX_AspNetUserRoles_RoleId" ON "AspNetUserRoles" ("RoleId");


CREATE INDEX "EmailIndex" ON "AspNetUsers" ("NormalizedEmail");


CREATE UNIQUE INDEX "UserNameIndex" ON "AspNetUsers" ("NormalizedUserName");


CREATE INDEX "IX_AutomatedTickets_UserId" ON "AutomatedTickets" ("UserId");


CREATE INDEX "IX_Balance_UserId" ON "Balance" ("UserId");


CREATE INDEX "IX_GameTickets_GameId" ON "GameTickets" ("GameId");


CREATE INDEX "IX_GameTickets_UserId" ON "GameTickets" ("UserId");
-- 
-- 
-- CREATE INDEX "IX_Payments_UserId" ON "Payments" ("UserId");

-- ALTER TABLE "AutomatedTickets"
--     ALTER COLUMN "GUID" SET DATA TYPE uuid
--         USING "GUID"::uuid;
-- 
-- ALTER TABLE "GameTickets"
--     ALTER COLUMN "GUID" SET DATA TYPE uuid
--         USING "GUID"::uuid;
-- 
-- -- Set the default value to uuid_generate_v4() for new rows
-- ALTER TABLE "AutomatedTickets"
--     ALTER COLUMN "GUID" SET DEFAULT uuid_generate_v4();
-- 
-- ALTER TABLE "GameTickets"
--     ALTER COLUMN "GUID" SET DEFAULT uuid_generate_v4();
-- 
-- ALTER TABLE "Balance"
--     ALTER COLUMN "GUID" SET DATA TYPE uuid
--         USING "GUID"::uuid;
-- 
-- ALTER TABLE "Balance"
--     ALTER COLUMN "GUID" SET DEFAULT uuid_generate_v4();




