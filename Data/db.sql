-- SQLite schema for Contoso.Mail

-- Clean up any existing tables with the same names
DROP TABLE IF EXISTS messages;
DROP TABLE IF EXISTS broadcasts;
DROP TABLE IF EXISTS emails;
DROP TABLE IF EXISTS subscriptions;
DROP TABLE IF EXISTS sequences;
DROP TABLE IF EXISTS tagged;
DROP TABLE IF EXISTS tags;
DROP TABLE IF EXISTS activity;
DROP TABLE IF EXISTS contacts;

-- Create tables
CREATE TABLE contacts (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  email TEXT NOT NULL UNIQUE,
  key TEXT NOT NULL DEFAULT (hex(randomblob(16))),
  subscribed BOOLEAN NOT NULL DEFAULT 1,
  name TEXT,
  created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE activity (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  contact_id INTEGER NOT NULL,
  key TEXT NOT NULL,
  description TEXT NOT NULL,
  created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  FOREIGN KEY (contact_id) REFERENCES contacts(id)
);

CREATE TABLE tags (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  slug TEXT NOT NULL UNIQUE,
  name TEXT,
  description TEXT,
  created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE tagged (
  contact_id INTEGER NOT NULL,
  tag_id INTEGER NOT NULL,
  PRIMARY KEY (contact_id, tag_id),
  FOREIGN KEY (contact_id) REFERENCES contacts(id),
  FOREIGN KEY (tag_id) REFERENCES tags(id)
);

CREATE TABLE sequences (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  slug TEXT NOT NULL UNIQUE,
  name TEXT,
  description TEXT,
  created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- people that are subscribed to a sequence
CREATE TABLE subscriptions (
  contact_id INTEGER NOT NULL,
  sequence_id INTEGER NOT NULL,
  created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (contact_id, sequence_id),
  FOREIGN KEY (contact_id) REFERENCES contacts(id),
  FOREIGN KEY (sequence_id) REFERENCES sequences(id)
);

-- templates, which can belong to 0/n sequences
CREATE TABLE emails (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  sequence_id INTEGER,
  slug TEXT NOT NULL UNIQUE,
  subject TEXT NOT NULL,
  preview TEXT,
  delay_hours INTEGER NOT NULL DEFAULT 0,
  html TEXT,
  created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  FOREIGN KEY (sequence_id) REFERENCES sequences(id)
);

CREATE TABLE broadcasts (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  email_id INTEGER NOT NULL,
  slug TEXT NOT NULL UNIQUE,
  status TEXT NOT NULL DEFAULT 'pending',
  name TEXT NOT NULL,
  send_to_tag TEXT,
  reply_to TEXT NOT NULL DEFAULT 'noreply@contosotraders.dev',
  created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  processed_at TIMESTAMP,
  FOREIGN KEY (email_id) REFERENCES emails(id)
);

-- This is a log table of actual emails sent
CREATE TABLE messages (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  source TEXT NOT NULL DEFAULT 'broadcast',
  slug TEXT,
  status TEXT NOT NULL DEFAULT 'pending',
  send_to TEXT NOT NULL,
  send_from TEXT NOT NULL,
  subject TEXT NOT NULL,
  html TEXT NOT NULL,
  send_at TIMESTAMP,
  sent_at TIMESTAMP,
  created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);