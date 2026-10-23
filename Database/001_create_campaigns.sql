create table campaigns
(
	id integer generated always as identity primary key,
	name varchar(128) not null,
	system varchar(128) not null,
	status varchar(16) not null check (status in('Aktywna','Wygasła'))
)