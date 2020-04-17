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