import json
import psycopg2
from faker import Faker
import random
from datetime import datetime

# Initialize Faker
fake = Faker()

# Function to generate South African ID numbers
def generate_south_african_id():
    birth_date = fake.date_of_birth(minimum_age=18, maximum_age=90)
    yy = birth_date.strftime('%y')
    mm = birth_date.strftime('%m')
    dd = birth_date.strftime('%d')
    ssss = str(random.randint(0, 9999)).zfill(4)
    c = str(random.randint(0, 4)) if random.choice([True, False]) else str(random.randint(5, 9))
    a = '0'  # assuming SA citizen
    z = '8'  # checksum digit, simplified for this example
    
    return f"{yy}{mm}{dd}{ssss}{c}{a}{z}"

# Number of dummy records you want to create
num_records = 20

# List to hold generated dummy data
dummy_data = []

for _ in range(num_records):
    record = {
        "id":  generate_south_african_id(),
        "first_name": fake.first_name(),
        "surname": fake.last_name(),
        "email": fake.email(),
        "join_date": fake.date_between(start_date='-2y', end_date='today').strftime('%Y-%m-%d'),
        "last_active_date": fake.date_between(start_date='-1y', end_date='today').strftime('%Y-%m-%d'),
        "converted": random.choice([False])
    }
    dummy_data.append(record)

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

# Insert dummy data into the existing table "prospects"
insert_query = '''
INSERT INTO public.prospects ("id", "first_name", "surname", "email", "join_date", "last_active_date", "converted")
VALUES (%s, %s, %s, %s, %s, %s, %s)
'''

for record in dummy_data:
    cur.execute(insert_query, (
        record['id'],
        record['first_name'],
        record['surname'],
        record['email'],
        record['join_date'],
        record['last_active_date'],
        record['converted']
    ))

# Commit the transaction
conn.commit()

# Close the cursor and connection
cur.close()
conn.close()

print("Dummy data inserted successfully.")
