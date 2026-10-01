--------------------------------------------------------------------
-- Preliminary query to determine (a) the values of, and (b) the
-- number of unique values for each key in the raw GCD data (JSONB).
-- TODO: LLM? data build tool (dbt)?
--------------------------------------------------------------------
WITH gcd_series AS (
    SELECT gcd_id, data
    FROM "Raw_GCD_Data"
    WHERE resource = 'series'
),
     gcd_series_keys AS (
         SELECT
             gcd_series.data->>'name'::text AS "Name"
--         gcd_series.data ->> 'api_url' AS "API Url",
--         gcd_series.data ->> 'year_began' AS "Year Began",
--         gcd_series.data ->> 'year_ended' AS "Year Ended",
--         gcd_series.data -> 'active_issues' AS "Active Issues",
--         gcd_series.data -> 'issue_descriptors' AS "Issue Descriptors",
--         gcd_series.data ->> 'country' AS "Country",
--         gcd_series.data ->> 'language' AS "Language",
--         gcd_series.data ->> 'publisher' AS "Publisher",
--         gcd_series.data -> 'publishing_format' AS "Publishing Format",
--         gcd_series.data ->> 'binding' AS "Binding",
--         gcd_series.data ->> 'color' AS "Color",
--         gcd_series.data ->> 'dimensions' AS "Dimensions",
--         gcd_series.data ->> 'paper_stock' AS "Paper Stock",
--         gcd_series.data ->> 'notes' AS "Notes"
FROM gcd_series
    ),
    distinct_series_names AS (
SELECT DISTINCT
    gsk."Name" AS "Distinct Series Names",
    COUNT(*) AS "Count"
FROM gcd_series_keys gsk
GROUP BY "Distinct Series Names"
ORDER BY gsk."Name" DESC
    )
SELECT
    "Distinct Series Names",
    SUM("Count") OVER() AS "Total Distinct Names"
FROM distinct_series_names AS dsn
GROUP BY
    "Distinct Series Names",
    "Count";
--------------------------------------------------------------------