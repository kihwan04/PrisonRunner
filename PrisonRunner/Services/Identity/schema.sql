CREATE TABLE IF NOT EXISTS profiles (
    id TEXT PRIMARY KEY,
    name TEXT NOT NULL,
    created_at INTEGER NOT NULL,
    consent_version TEXT NOT NULL
);
CREATE TABLE IF NOT EXISTS face_samples (
    profile_id TEXT NOT NULL REFERENCES profiles(id) ON DELETE CASCADE,
    embedding BLOB NOT NULL,
    PRIMARY KEY(profile_id, embedding)
);
CREATE TABLE IF NOT EXISTS sessions (
    token_hash TEXT PRIMARY KEY,
    profile_id TEXT NOT NULL REFERENCES profiles(id) ON DELETE CASCADE,
    expires_at INTEGER NOT NULL
);
CREATE TABLE IF NOT EXISTS runs (
    id TEXT PRIMARY KEY,
    profile_id TEXT NOT NULL REFERENCES profiles(id) ON DELETE CASCADE,
    distance REAL NOT NULL CHECK(distance >= 0),
    score INTEGER NOT NULL CHECK(score >= 0),
    kcal REAL NOT NULL CHECK(kcal >= 0),
    goal REAL NOT NULL CHECK(goal > 0),
    activities TEXT NOT NULL,
    created_at INTEGER NOT NULL
);
CREATE INDEX IF NOT EXISTS runs_profile_created ON runs(profile_id, created_at DESC);
CREATE INDEX IF NOT EXISTS sessions_expiry ON sessions(expires_at);
PRAGMA user_version=1;
