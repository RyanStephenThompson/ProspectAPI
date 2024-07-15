import psycopg2
import pandas as pd
import random
from faker import Faker

# Initialize Faker
fake = Faker()

# Load the rewards and prospects CSV files
rewards_df = pd.read_csv('rewards.csv')
prospects_df = pd.read_csv('prospects.csv')

# Extract relevant columns
rewards_IDs = rewards_df['R_ID'].tolist()
rewards_Categories = rewards_df['Category'].tolist()
prospects_list = prospects_df['DDID'].tolist()

# Number of dummy redemption records you want to create
num_redemptions = 10  # You can adjust this number

# List to hold generated dummy redemptions data
dummy_redemptions = []

for _ in range(num_redemptions):
    record = {
        "R_ID": random.choice(rewards_IDs),  # Random R_ID
        "DDID": random.choice(prospects_list),
        "Category": random.choice(rewards_Categories),
        "Date": fake.date_between(start_date='-1y', end_date='today').strftime('%Y-%m-%d')
    }
    dummy_redemptions.append(record)

# Convert the list of dictionaries to a DataFrame
redemptions_df = pd.DataFrame(dummy_redemptions)

# PostgreSQL database connection details
db_config = {
    'dbname': 'prospect_management',
    'user': 'postgres',
    'password': 'Ryan2000',
    'host': 'localhost',
    'port': '5432'  # default is 5432
}

# Connect to PostgreSQL database
conn = psycopg2.connect(**db_config)
cur = conn.cursor()

# Create the redemptions table if it doesn't exist
create_table_query = '''
CREATE TABLE IF NOT EXISTS public.redemptions
(
    "R_ID" integer NOT NULL,
    "DDID" integer NOT NULL,
    "Category" text COLLATE pg_catalog."default",
    "Date" date,
    CONSTRAINT "Redemptions_pkey" PRIMARY KEY ("R_ID", "DDID")
)
'''
cur.execute(create_table_query)
conn.commit()

# Insert dummy redemptions data into the redemptions table
insert_query = '''
INSERT INTO public.redemptions ("R_ID", "DDID", "Category", "Date")
VALUES (%s, %s, %s, %s)
'''

for _, record in redemptions_df.iterrows():
    cur.execute(insert_query, (
        record['R_ID'],
        record['DDID'],
        record['Category'],
        record['Date']
    ))

# Commit the transaction
conn.commit()

# Close the cursor and connection
cur.close()
conn.close()

print("Dummy redemptions data inserted successfully into redemptions table.")
