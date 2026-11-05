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