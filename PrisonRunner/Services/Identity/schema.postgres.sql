CREATE TABLE IF NOT EXISTS profiles (
    id TEXT PRIMARY KEY,
    name TEXT NOT NULL,
    avatar BYTEA NOT NULL,
    created_at BIGINT NOT NULL,
    consent_version TEXT NOT NULL
);
CREATE TABLE IF NOT EXISTS face_samples (
    profile_id TEXT NOT NULL REFERENCES profiles(id) ON DELETE CASCADE,
    embedding BYTEA NOT NULL,
    PRIMARY KEY (profile_id, embedding)
);
CREATE TABLE IF NOT EXISTS sessions (
    token_hash TEXT PRIMARY KEY,
    profile_id TEXT NOT NULL REFERENCES profiles(id) ON DELETE CASCADE,
    expires_at BIGINT NOT NULL
);
CREATE TABLE IF NOT EXISTS runs (
    id TEXT PRIMARY KEY,
    profile_id TEXT NOT NULL REFERENCES profiles(id) ON DELETE CASCADE,
    distance DOUBLE PRECISION NOT NULL CHECK (distance >= 0),
    score BIGINT NOT NULL CHECK (score >= 0),
    kcal DOUBLE PRECISION NOT NULL CHECK (kcal >= 0),
    goal DOUBLE PRECISION NOT NULL CHECK (goal > 0),
    activities JSONB NOT NULL CHECK (jsonb_typeof(activities) = 'array' AND jsonb_array_length(activities) = 5),
    created_at BIGINT NOT NULL
);
CREATE INDEX IF NOT EXISTS runs_profile_created ON runs (profile_id, created_at DESC);
CREATE INDEX IF NOT EXISTS sessions_expiry ON sessions (expires_at);
CREATE INDEX IF NOT EXISTS sessions_profile ON sessions (profile_id);
