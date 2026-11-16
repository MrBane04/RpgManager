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
);
SELECT campaign_id, COUNT(*)
FROM sessions
GROUP BY campaign_id
ORDER BY campaign_id;

INSERT INTO sessions
    (campaign_id, title, description, session_date, duration_minutes, status)
VALUES
    (7, 'Sesja 1', 'Początek przygody', NOW() - INTERVAL '15 days', 180, 'completed'),
    (7, 'Sesja 2', 'Spotkanie z kupcem', NOW() - INTERVAL '14 days', 150, 'completed'),
    (7, 'Sesja 3', 'Podróż przez las', NOW() - INTERVAL '13 days', 210, 'completed'),
    (7, 'Sesja 4', 'Tajemnicza jaskinia', NOW() - INTERVAL '12 days', 180, 'completed'),
    (7, 'Sesja 5', 'Walka z goblinami', NOW() - INTERVAL '11 days', 120, 'completed'),
    (7, 'Sesja 6', 'Zaginiony artefakt', NOW() - INTERVAL '10 days', 180, 'completed'),
    (7, 'Sesja 7', 'Ruiny starego miasta', NOW() - INTERVAL '9 days', 240, 'completed'),
    (7, 'Sesja 8', 'Tajemniczy nieznajomy', NOW() - INTERVAL '8 days', 150, 'completed'),
    (7, 'Sesja 9', 'Atak bandytów', NOW() - INTERVAL '7 days', 180, 'completed'),
    (7, 'Sesja 10', 'Przeprawa przez góry', NOW() - INTERVAL '6 days', 210, 'completed'),
    (7, 'Sesja 11', 'Spotkanie z magiem', NOW() + INTERVAL '1 day', 180, 'planned'),
    (7, 'Sesja 12', 'Podziemne lochy', NOW() + INTERVAL '3 days', 240, 'planned'),
    (7, 'Sesja 13', 'Wielki turniej', NOW() + INTERVAL '5 days', 180, 'planned'),
    (7, 'Sesja 14', 'Ostatnia przepowiednia', NOW() + INTERVAL '7 days', 210, 'planned'),
    (7, 'Sesja 15', 'Finał przygody', NOW() + INTERVAL '10 days', 240, 'planned');