CREATE TABLE IF NOT EXISTS 
    keyvalues
    (
        id    uuid           not null
                primary key,
        key   varchar(10000) not null,
        value varchar(10000) not null
    );

CREATE TABLE IF NOT EXISTS users(
        id uuid NOT NULL primary key,
        email varchar(10000) NOT NULL,
        password varchar(10000) NOT NULL
        
    );

CREATE TABLE IF NOT EXISTS ImportResults(
    Id uuid NOT NULL primary key,
    SessionId uuid NOT NULL,
    UserId uuid NOT NULL,
    Status int NOT NULL,
    Date timestamp NOT NULL,
    Title varchar NOT NULL,
    Artist varchar NOT NULL,
    NormalizedTitle varchar NOT NULL,
    NormalizedArtist varchar NOT NULL,
    CandidateTitle varchar NOT NULL,
    CandidateArtist varchar NOT NULL,
    Message varchar NOT NULL
);

CREATE TABLE IF NOT EXISTS playlists(
    Id uuid NOT NULL primary key,
    UserId uuid NOT NULL,
    Content varchar NOT NULL,
    Filename varchar NOT NULL,
    Extension varchar NOT NULL,
    Date timestamp NOT NULL
);

CREATE TABLE IF NOT EXISTS tempsessions(
    Id uuid NOT NULL primary key,
    UserId uuid NULL,
    Text varchar NOT NULL,
    Date timestamp NOT NULL
);

CREATE INDEX IF NOT EXISTS importresults_sessionid on ImportResults (SessionId);
CREATE INDEX IF NOT EXISTS importresults_userid on ImportResults (UserId);
CREATE INDEX IF NOT EXISTS importresults_date on ImportResults (Date);

ALTER TABLE ImportResults ADD COLUMN IF NOT EXISTS approvedmanual boolean default false;
ALTER TABLE ImportResults ADD COLUMN IF NOT EXISTS candidatealbumid varchar NULL;
ALTER TABLE ImportResults ADD COLUMN IF NOT EXISTS candidatetrackid varchar NULL;
ALTER TABLE ImportResults ADD COLUMN IF NOT EXISTS playlistid varchar NULL;

CREATE TABLE IF NOT EXISTS spotify_tokens(
                                             user_id uuid PRIMARY KEY NOT NULL,
                                             access_token varchar NOT NULL,
                                             token_type varchar NOT NULL,
                                             scope varchar NOT NULL,
                                             expires_utc timestamp NULL,
                                             refresh_token varchar NOT NULL
);

ALTER TABLE spotify_tokens ALTER expires_utc TYPE timestamptz USING expires_utc AT TIME ZONE 'UTC';
ALTER TABLE tempsessions ALTER Date TYPE timestamptz USING Date AT TIME ZONE 'UTC';
ALTER TABLE playlists ALTER Date TYPE timestamptz USING Date AT TIME ZONE 'UTC';
ALTER TABLE ImportResults ALTER Date TYPE timestamptz USING Date AT TIME ZONE 'UTC';