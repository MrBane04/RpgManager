create table notes
(
	id integer generated always as identity primary key,
	campaign_id integer not null unique,
	content text not null default '',
	created_at timestamptz not null default now(),
	updated_at timestamptz not null default now(),
	constraint fk_campaign_notes foreign key (campaign_id) references campaigns(id) on delete cascade
)