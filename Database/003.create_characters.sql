create table characters
(
	id integer generated always as identity primary key,
	campaign_id integer not null,
	name varchar(128) not null,
	age integer not null check(age>0),
	race varchar(64),
	character_class varchar(64),
	level integer not null default 1 check(level>0),
	description text,
	created_at timestamptz not null default now(),
	constraint fk_character_campaign foreign key(campaign_id) references campaigns(id) on delete cascade
);

create index idx_character_campaign_id
on characters(campaign_id);


INSERT INTO characters
    (campaign_id, name, age, race, character_class, level, description)
SELECT
    (SELECT id FROM campaigns ORDER BY id LIMIT 1),
    name,
    age,
    race,
    character_class,
    level,
    description
FROM (VALUES
    ('Aragorn', 87, 'Człowiek', 'Wojownik', 10, 'Dziedzic Isildura'),
    ('Legolas', 2931, 'Elf', 'Łucznik', 12, 'Książę Mrocznej Puszczy'),
    ('Gimli', 139, 'Krasnolud', 'Wojownik', 9, 'Syn Gloina'),
    ('Gandalf', 2019, 'Maia', 'Czarodziej', 20, 'Czarodziej wędrowiec'),
    ('Frodo', 33, 'Hobbit', 'Łotrzyk', 5, 'Powiernik Jedynego Pierścienia'),
    ('Samwise', 38, 'Hobbit', 'Wojownik', 4, 'Wierny towarzysz Froda'),
    ('Boromir', 41, 'Człowiek', 'Wojownik', 8, 'Syn namiestnika Gondoru'),
    ('Eowina', 24, 'Człowiek', 'Paladyn', 7, 'Wojowniczka Rohanu'),
    ('Gollum', 589, 'Hobbit', 'Łotrzyk', 6, 'Dawny powiernik pierścienia'),
    ('Galadriela', 8372, 'Elf', 'Czarodziej', 18, 'Pani Lothlorien'),
    ('Gimli Junior', 25, 'Krasnolud', 'Wojownik', 3, 'Młody krasnolud'),
    ('Thorin', 195, 'Krasnolud', 'Wojownik', 11, 'Syn Thráina'),
    ('Bard', 45, 'Człowiek', 'Łucznik', 8, 'Łucznik z Miasta na Jeziorze'),
    ('Radagast', 1500, 'Maia', 'Druid', 15, 'Opiekun natury'),
    ('Elrond', 6520, 'Elf', 'Czarodziej', 16, 'Władca Rivendell')
) AS characters(name, age, race, character_class, level, description);