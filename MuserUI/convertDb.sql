CREATE TABLE IF NOT EXISTS 
    keyvalues
    (
        id    uuid           not null
                primary key,
        key   varchar(10000) not null,
        value varchar(10000) not null
    );