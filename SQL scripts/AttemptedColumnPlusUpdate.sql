ALTER TABLE Exercise ADD Attempted bit NOT NULL DEFAULT 0
UPDATE Exercise SET Exercise.Attempted = 1 WHERE  (Score > 0)
