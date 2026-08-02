# Unity Rules

-   Never use GameObject.Find.
-   Never use FindObjectOfType unless approved.
-   Never use Resources.Load.
-   Cache references in Awake().
-   Physics in FixedUpdate().
-   Input in Update().
-   Configure gameplay values through Inspector.
-   Avoid allocations every frame.
-   No LINQ inside Update/FixedUpdate.
-   No Singleton unless explicitly approved.
