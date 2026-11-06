CREATE TABLE sessions 
(
	id integer generated always as identity primary key,
	campaign_id int NOT NULL,
	title varchar(128) NOT NULL,
	description text,
	session_date timestamptz NOT NULL,
	duration_minutes int CHECK(duration_minutes > 0),
	status varchar(16) NOT NULL CHECK(status IN('planned','completed','canceled')),
	created_at timestamptz NOT NULL DEFAULT now(),
	CONSTRAINT fk_session_campaign FOREIGN KEY (campaign_id) REFERENCES campaigns(id) ON DELETE CASCADE
)


