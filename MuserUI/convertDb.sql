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
        
    )

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
)