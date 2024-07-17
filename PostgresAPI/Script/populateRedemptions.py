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
rewards_IDs = rewards_df['reward_id'].tolist()
rewards_Categories = rewards_df['category'].tolist()
prospects_list = prospects_df['ddid'].tolist()

# Number of dummy redemption records you want to create
num_redemptions = 30  # You can adjust this number

# List to hold generated dummy redemptions data
dummy_redemptions = []

for _ in range(num_redemptions):
    record = {
        "reward_id": random.choice(rewards_IDs),  # Random R_ID
        "ddid": random.choice(prospects_list),
        "category": random.choice(rewards_Categories),
        "date": fake.date_between(start_date='-1y', end_date='today').strftime('%Y-%m-%d')
    }
    dummy_redemptions.append(record)

# Convert the list of dictionaries to a DataFrame
redemptions_df = pd.DataFrame(dummy_redemptions)

# PostgreSQL database connection details
db_config = {
    'dbname': 'postgres',
    'user': 'postgres.tvymgmpnqrhgngstcppl',
    'password': '2Vtp!!n$F5fxtFX',
    'host': 'aws-0-eu-west-2.pooler.supabase.com',
    'port': '6543'  # default is 5432
}

# Connect to PostgreSQL database
conn = psycopg2.connect(**db_config)
cur = conn.cursor()

# Insert dummy redemptions data into the redemptions table
insert_query = '''
INSERT INTO public.redemptions ("ddid", "category", "date","reward_id")
VALUES (%s, %s, %s, %s)
'''

for _, record in redemptions_df.iterrows():
    cur.execute(insert_query, (       
        record['ddid'],
        record['category'],
        record['date'],
        record['reward_id']
    ))

# Commit the transaction
conn.commit()

# Close the cursor and connection
cur.close()
conn.close()

print("Dummy redemptions data inserted successfully into redemptions table.")
