-- =============================================
-- SEED DATA FOR ONLINE LEARNING PLATFORM
-- Run this after database migration
-- Column names are snake_case as per DbContext
-- Prices in VND, Total 20 courses
-- Only uses 5 provided image URLs
-- =============================================

-- Clear existing data (in correct order to handle foreign keys)
DELETE FROM [Options];
DELETE FROM [Questions];
DELETE FROM [Quizzes];
DELETE FROM [Lesson_Progress];
DELETE FROM [Lesson_Comments];
DELETE FROM [Lessons];
DELETE FROM [Modules];
DELETE FROM [Path_Courses];
DELETE FROM [Learning_Paths];
DELETE FROM [Enrollments];
DELETE FROM [Wishlists];
DELETE FROM [Course_Reviews];
DELETE FROM [Transactions];
DELETE FROM [Orders];
DELETE FROM [Courses];
DELETE FROM [Categories];
DELETE FROM [User_Assessments];
DELETE FROM [profiles];
DELETE FROM [Notifications];
DELETE FROM [users];
DELETE FROM [roles];

-- =============================================
-- 1. ROLES
-- =============================================
DECLARE @AdminRoleId UNIQUEIDENTIFIER = NEWID()
DECLARE @InstructorRoleId UNIQUEIDENTIFIER = NEWID()
DECLARE @UserRoleId UNIQUEIDENTIFIER = NEWID()

INSERT INTO [roles] ([Id], [Name], [Description]) VALUES
(@AdminRoleId, 'Admin', 'Administrator with full system access'),
(@InstructorRoleId, 'Instructor', 'Course instructor who can create and manage courses'),
(@UserRoleId, 'User', 'Regular learner who can enroll in courses');

-- =============================================
-- 2. USERS
-- Password is 'Password123!' hashed with BCrypt
-- =============================================
DECLARE @AdminId UNIQUEIDENTIFIER = NEWID()
DECLARE @Instructor1Id UNIQUEIDENTIFIER = NEWID()
DECLARE @Instructor2Id UNIQUEIDENTIFIER = NEWID()
DECLARE @Instructor3Id UNIQUEIDENTIFIER = NEWID()
DECLARE @Instructor4Id UNIQUEIDENTIFIER = NEWID()
DECLARE @Student1Id UNIQUEIDENTIFIER = NEWID()
DECLARE @Student2Id UNIQUEIDENTIFIER = NEWID()
DECLARE @Student3Id UNIQUEIDENTIFIER = NEWID()

-- BCrypt hash for 'Password123!'
DECLARE @PasswordHash NVARCHAR(100) = '$2a$11$rBNdBxYU5sVEm4q9qwABkuNjh4BjY5RmZxB.1HbJvSZMVvxFCz0z6'

INSERT INTO [users] ([Id], [Username], [Email], [PasswordHash], [created_at], [RoleId], [is_active], [has_completed_assessment]) VALUES
(@AdminId, 'admin', 'admin@learnhub.com', @PasswordHash, GETUTCDATE(), @AdminRoleId, 1, 0),
(@Instructor1Id, 'john.smith', 'john.smith@learnhub.com', @PasswordHash, GETUTCDATE(), @InstructorRoleId, 1, 1),
(@Instructor2Id, 'jane.doe', 'jane.doe@learnhub.com', @PasswordHash, GETUTCDATE(), @InstructorRoleId, 1, 1),
(@Instructor3Id, 'mike.wilson', 'mike.wilson@learnhub.com', @PasswordHash, GETUTCDATE(), @InstructorRoleId, 1, 1),
(@Instructor4Id, 'sarah.lee', 'sarah.lee@learnhub.com', @PasswordHash, GETUTCDATE(), @InstructorRoleId, 1, 1),
(@Student1Id, 'alice.nguyen', 'alice.nguyen@gmail.com', @PasswordHash, GETUTCDATE(), @UserRoleId, 1, 1),
(@Student2Id, 'bob.tran', 'bob.tran@gmail.com', @PasswordHash, GETUTCDATE(), @UserRoleId, 1, 0),
(@Student3Id, 'carol.le', 'carol.le@gmail.com', @PasswordHash, GETUTCDATE(), @UserRoleId, 1, 1);

-- =============================================
-- 3. PROFILES
-- =============================================
INSERT INTO [profiles] ([Id], [UserId], [FirstName], [LastName], [AvatarUrl], [Description]) VALUES
(NEWID(), @AdminId, 'System', 'Admin', 'https://ui-avatars.com/api/?name=Admin&background=6366f1&color=fff', 'Platform administrator'),
(NEWID(), @Instructor1Id, 'John', 'Smith', 'https://ui-avatars.com/api/?name=John+Smith&background=10b981&color=fff', 'Senior .NET Developer with 10+ years experience. Microsoft MVP.'),
(NEWID(), @Instructor2Id, 'Jane', 'Doe', 'https://ui-avatars.com/api/?name=Jane+Doe&background=f59e0b&color=fff', 'Full-stack developer specializing in Python and Data Science.'),
(NEWID(), @Instructor3Id, 'Mike', 'Wilson', 'https://ui-avatars.com/api/?name=Mike+Wilson&background=ef4444&color=fff', 'DevOps engineer and cloud architecture expert.'),
(NEWID(), @Instructor4Id, 'Sarah', 'Lee', 'https://ui-avatars.com/api/?name=Sarah+Lee&background=3b82f6&color=fff', 'Mobile developer with expertise in React Native and Flutter.'),
(NEWID(), @Student1Id, 'Alice', 'Nguyen', 'https://ui-avatars.com/api/?name=Alice+Nguyen&background=8b5cf6&color=fff', 'Computer Science student passionate about web development.'),
(NEWID(), @Student2Id, 'Bob', 'Tran', 'https://ui-avatars.com/api/?name=Bob+Tran&background=06b6d4&color=fff', 'Aspiring data scientist looking to transition careers.'),
(NEWID(), @Student3Id, 'Carol', 'Le', 'https://ui-avatars.com/api/?name=Carol+Le&background=ec4899&color=fff', 'UX designer learning to code.');

-- =============================================
-- 4. CATEGORIES
-- =============================================
DECLARE @WebDevCategory UNIQUEIDENTIFIER = NEWID()
DECLARE @BackendCategory UNIQUEIDENTIFIER = NEWID()
DECLARE @DataCategory UNIQUEIDENTIFIER = NEWID()
DECLARE @DatabaseCategory UNIQUEIDENTIFIER = NEWID()
DECLARE @FrontendCategory UNIQUEIDENTIFIER = NEWID()
DECLARE @MobileCategory UNIQUEIDENTIFIER = NEWID()
DECLARE @DevOpsCategory UNIQUEIDENTIFIER = NEWID()

INSERT INTO [Categories] ([category_id], [name], [parent_id], [description]) VALUES
(@WebDevCategory, 'Web Development', NULL, 'Learn to build modern web applications'),
(@BackendCategory, 'Backend Development', NULL, 'Server-side programming and APIs'),
(@DataCategory, 'Data Science', NULL, 'Data analysis, machine learning, and AI'),
(@DatabaseCategory, 'Database', NULL, 'Database design and management'),
(@FrontendCategory, 'Frontend Development', NULL, 'User interface and user experience'),
(@MobileCategory, 'Mobile Development', NULL, 'iOS, Android, and cross-platform apps'),
(@DevOpsCategory, 'DevOps & Cloud', NULL, 'CI/CD, Docker, Kubernetes, and Cloud services');

-- =============================================
-- 5. COURSES (20 COURSES - Prices in VND)
-- Image URLs: net.jpg, node.png, python.jpg, sql.png, html.jpg
-- =============================================
DECLARE @Course1Id UNIQUEIDENTIFIER = NEWID()
DECLARE @Course2Id UNIQUEIDENTIFIER = NEWID()
DECLARE @Course3Id UNIQUEIDENTIFIER = NEWID()
DECLARE @Course4Id UNIQUEIDENTIFIER = NEWID()
DECLARE @Course5Id UNIQUEIDENTIFIER = NEWID()
DECLARE @Course6Id UNIQUEIDENTIFIER = NEWID()
DECLARE @Course7Id UNIQUEIDENTIFIER = NEWID()
DECLARE @Course8Id UNIQUEIDENTIFIER = NEWID()
DECLARE @Course9Id UNIQUEIDENTIFIER = NEWID()
DECLARE @Course10Id UNIQUEIDENTIFIER = NEWID()
DECLARE @Course11Id UNIQUEIDENTIFIER = NEWID()
DECLARE @Course12Id UNIQUEIDENTIFIER = NEWID()
DECLARE @Course13Id UNIQUEIDENTIFIER = NEWID()
DECLARE @Course14Id UNIQUEIDENTIFIER = NEWID()
DECLARE @Course15Id UNIQUEIDENTIFIER = NEWID()
DECLARE @Course16Id UNIQUEIDENTIFIER = NEWID()
DECLARE @Course17Id UNIQUEIDENTIFIER = NEWID()
DECLARE @Course18Id UNIQUEIDENTIFIER = NEWID()
DECLARE @Course19Id UNIQUEIDENTIFIER = NEWID()
DECLARE @Course20Id UNIQUEIDENTIFIER = NEWID()

-- Image URLs
DECLARE @NetImg NVARCHAR(200) = 'https://online-learning-platform.sfo3.cdn.digitaloceanspaces.com/net.jpg'
DECLARE @NodeImg NVARCHAR(200) = 'https://online-learning-platform.sfo3.cdn.digitaloceanspaces.com/node.png'
DECLARE @PythonImg NVARCHAR(200) = 'https://online-learning-platform.sfo3.cdn.digitaloceanspaces.com/python.jpg'
DECLARE @SqlImg NVARCHAR(200) = 'https://online-learning-platform.sfo3.cdn.digitaloceanspaces.com/sql.png'
DECLARE @HtmlImg NVARCHAR(200) = 'https://online-learning-platform.sfo3.cdn.digitaloceanspaces.com/html.jpg'

INSERT INTO [Courses] ([course_id], [instructor_id], [category_id], [title], [description], [price], [status], [created_at], [image_url], [is_featured], [level], [language]) VALUES
-- Backend Development courses (use net.jpg, node.png)
(@Course1Id, @Instructor1Id, @BackendCategory, 'Complete ASP.NET Core MVC Developer Course', 
'Master ASP.NET Core MVC from scratch! Learn to build professional web applications with C#, Entity Framework Core, Identity, and deploy to Azure.', 
599000, 'Published', DATEADD(DAY, -30, GETUTCDATE()), @NetImg, 1, 'Intermediate', 'English'),

(@Course2Id, @Instructor1Id, @BackendCategory, 'Node.js - The Complete Guide', 
'Build scalable backend applications with Node.js, Express, and MongoDB. Learn REST APIs, GraphQL, authentication with JWT.', 
499000, 'Published', DATEADD(DAY, -28, GETUTCDATE()), @NodeImg, 1, 'Beginner', 'English'),

(@Course6Id, @Instructor1Id, @BackendCategory, 'Advanced ASP.NET Core Web API', 
'Build enterprise-grade RESTful APIs with ASP.NET Core. Learn authentication, caching, versioning, and deployment.', 
699000, 'Published', DATEADD(DAY, -22, GETUTCDATE()), @NetImg, 1, 'Advanced', 'English'),

(@Course7Id, @Instructor2Id, @BackendCategory, 'Node.js Microservices Architecture', 
'Master microservices with Node.js. Build scalable, distributed systems with message queues and Docker.', 
549000, 'Published', DATEADD(DAY, -20, GETUTCDATE()), @NodeImg, 0, 'Advanced', 'English'),

(@Course8Id, @Instructor1Id, @BackendCategory, 'ASP.NET Core Blazor Complete Guide', 
'Build interactive web UIs with Blazor WebAssembly and Blazor Server. Create SPA applications with C#.', 
449000, 'Published', DATEADD(DAY, -18, GETUTCDATE()), @NetImg, 0, 'Intermediate', 'English'),

-- Data Science courses (use python.jpg)
(@Course3Id, @Instructor2Id, @DataCategory, 'Python for Data Science and Machine Learning', 
'Comprehensive Python course for data science! Master NumPy, Pandas, Matplotlib, Scikit-Learn, and TensorFlow.', 
799000, 'Published', DATEADD(DAY, -26, GETUTCDATE()), @PythonImg, 1, 'Intermediate', 'English'),

(@Course9Id, @Instructor2Id, @DataCategory, 'Deep Learning with Python and Keras', 
'Build neural networks, CNNs, RNNs, and transformers. Learn computer vision and NLP with real-world projects.', 
899000, 'Published', DATEADD(DAY, -16, GETUTCDATE()), @PythonImg, 1, 'Advanced', 'English'),

(@Course10Id, @Instructor2Id, @DataCategory, 'Python Data Analysis Bootcamp', 
'Master data analysis with Python. Learn Pandas, data visualization, and statistical analysis.', 
449000, 'Published', DATEADD(DAY, -14, GETUTCDATE()), @PythonImg, 0, 'Beginner', 'English'),

-- Database courses (use sql.png)
(@Course4Id, @Instructor3Id, @DatabaseCategory, 'SQL Masterclass: Complete Database Management', 
'From zero to SQL hero! Learn SQL Server, PostgreSQL, and MySQL. Master queries, joins, stored procedures.', 
449000, 'Published', DATEADD(DAY, -24, GETUTCDATE()), @SqlImg, 1, 'Beginner', 'English'),

(@Course11Id, @Instructor3Id, @DatabaseCategory, 'Advanced SQL and Database Optimization', 
'Master advanced SQL techniques. Learn indexing, query optimization, and database performance tuning.', 
549000, 'Published', DATEADD(DAY, -12, GETUTCDATE()), @SqlImg, 0, 'Advanced', 'English'),

-- Frontend Development courses (use html.jpg, node.png for JS frameworks)
(@Course5Id, @Instructor2Id, @FrontendCategory, 'Modern HTML5 & CSS3 From Scratch', 
'Build beautiful, responsive websites from scratch! Learn HTML5 semantics, CSS3 animations, Flexbox, and CSS Grid.', 
299000, 'Published', DATEADD(DAY, -25, GETUTCDATE()), @HtmlImg, 1, 'Beginner', 'English'),

(@Course12Id, @Instructor4Id, @FrontendCategory, 'React - The Complete Guide 2024', 
'Build powerful single-page applications with React 18. Master hooks, Redux, React Router, and Next.js.', 
649000, 'Published', DATEADD(DAY, -10, GETUTCDATE()), @NodeImg, 1, 'Intermediate', 'English'),

(@Course13Id, @Instructor4Id, @FrontendCategory, 'Vue.js 3 Complete Course', 
'Learn Vue.js 3 from scratch. Master Composition API, Vuex, Vue Router, and build full-stack apps.', 
549000, 'Published', DATEADD(DAY, -8, GETUTCDATE()), @NodeImg, 0, 'Intermediate', 'English'),

(@Course14Id, @Instructor4Id, @FrontendCategory, 'Advanced CSS and Sass Masterclass', 
'Master advanced CSS techniques, Sass preprocessing, and modern CSS architecture patterns.', 
399000, 'Published', DATEADD(DAY, -6, GETUTCDATE()), @HtmlImg, 0, 'Intermediate', 'English'),

-- Mobile Development courses (use node.png for React Native, python.jpg for cross-platform)
(@Course15Id, @Instructor4Id, @MobileCategory, 'React Native - Build Mobile Apps', 
'Build cross-platform mobile apps with React Native. Deploy to iOS and Android with a single codebase.', 
699000, 'Published', DATEADD(DAY, -5, GETUTCDATE()), @NodeImg, 1, 'Intermediate', 'English'),

(@Course16Id, @Instructor4Id, @MobileCategory, 'Mobile App Development Fundamentals', 
'Learn mobile app development concepts. Understand UI/UX, navigation, and native components.', 
349000, 'Published', DATEADD(DAY, -4, GETUTCDATE()), @HtmlImg, 1, 'Beginner', 'English'),

(@Course17Id, @Instructor4Id, @MobileCategory, 'React Native Advanced Patterns', 
'Advanced React Native techniques. Learn performance optimization, animations, and native modules.', 
749000, 'Published', DATEADD(DAY, -3, GETUTCDATE()), @NodeImg, 0, 'Advanced', 'English'),

-- DevOps & Cloud courses (use net.jpg for enterprise, python.jpg for automation)
(@Course18Id, @Instructor3Id, @DevOpsCategory, 'Docker and Container Fundamentals', 
'Master containerization with Docker. Learn images, containers, networking, and Docker Compose.', 
599000, 'Published', DATEADD(DAY, -2, GETUTCDATE()), @NetImg, 1, 'Intermediate', 'English'),

(@Course19Id, @Instructor3Id, @DevOpsCategory, 'Cloud Architecture with Azure', 
'Learn Azure cloud services. Master VMs, App Services, Azure SQL, and cloud architecture patterns.', 
899000, 'Published', DATEADD(DAY, -1, GETUTCDATE()), @NetImg, 1, 'Advanced', 'English'),

(@Course20Id, @Instructor3Id, @DevOpsCategory, 'DevOps Automation with Python', 
'Automate your infrastructure with Python. Learn scripting, CI/CD pipelines, and infrastructure as code.', 
549000, 'Published', GETUTCDATE(), @PythonImg, 0, 'Intermediate', 'English');

-- =============================================
-- 6. MODULES (2 modules per first 5 courses)
-- =============================================
DECLARE @Module1_1 UNIQUEIDENTIFIER = NEWID()
DECLARE @Module1_2 UNIQUEIDENTIFIER = NEWID()
DECLARE @Module2_1 UNIQUEIDENTIFIER = NEWID()
DECLARE @Module2_2 UNIQUEIDENTIFIER = NEWID()
DECLARE @Module3_1 UNIQUEIDENTIFIER = NEWID()
DECLARE @Module3_2 UNIQUEIDENTIFIER = NEWID()
DECLARE @Module4_1 UNIQUEIDENTIFIER = NEWID()
DECLARE @Module4_2 UNIQUEIDENTIFIER = NEWID()
DECLARE @Module5_1 UNIQUEIDENTIFIER = NEWID()
DECLARE @Module5_2 UNIQUEIDENTIFIER = NEWID()

INSERT INTO [Modules] ([module_id], [course_id], [title], [order_index], [description], [created_at]) VALUES
(@Module1_1, @Course1Id, 'Getting Started with ASP.NET Core', 1, 'Introduction to .NET ecosystem and setting up development environment', GETUTCDATE()),
(@Module1_2, @Course1Id, 'MVC Pattern and Controllers', 2, 'Understanding Model-View-Controller architecture', GETUTCDATE()),
(@Module2_1, @Course2Id, 'Node.js Fundamentals', 1, 'Core concepts of Node.js and JavaScript runtime', GETUTCDATE()),
(@Module2_2, @Course2Id, 'Building REST APIs with Express', 2, 'Creating robust APIs with Express.js framework', GETUTCDATE()),
(@Module3_1, @Course3Id, 'Python Basics for Data Science', 1, 'Python fundamentals and development environment', GETUTCDATE()),
(@Module3_2, @Course3Id, 'Data Analysis with Pandas', 2, 'Mastering data manipulation with Pandas library', GETUTCDATE()),
(@Module4_1, @Course4Id, 'SQL Basics', 1, 'Introduction to SQL and database concepts', GETUTCDATE()),
(@Module4_2, @Course4Id, 'Advanced Queries', 2, 'Complex queries, joins, and subqueries', GETUTCDATE()),
(@Module5_1, @Course5Id, 'HTML5 Fundamentals', 1, 'Semantic HTML and document structure', GETUTCDATE()),
(@Module5_2, @Course5Id, 'CSS3 Styling', 2, 'Modern CSS techniques and layouts', GETUTCDATE());

-- =============================================
-- 7. LESSONS
-- =============================================
DECLARE @VideoUrl1 NVARCHAR(500) = 'https://online-learning-platform.sfo3.cdn.digitaloceanspaces.com/mvclagi.mp4'
DECLARE @VideoUrl2 NVARCHAR(500) = 'https://online-learning-platform.sfo3.cdn.digitaloceanspaces.com/whatisRestAPI.mp4'

DECLARE @ReadContent1 NVARCHAR(MAX) = N'<div class="lesson-content">
    <h1>Chào mừng đến với ASP.NET Core</h1>
    <p class="lead">ASP.NET Core là framework cross-platform, hiệu suất cao để xây dựng web applications hiện đại.</p>
    <h2>Lợi ích chính:</h2>
    <ul>
        <li><strong>Cross-platform:</strong> Chạy trên Windows, macOS, và Linux</li>
        <li><strong>Hiệu suất cao:</strong> Một trong những web frameworks nhanh nhất</li>
        <li><strong>Open-source:</strong> Được phát triển bởi cộng đồng</li>
    </ul>
    <pre><code class="language-bash">dotnet new mvc -n MyFirstApp
cd MyFirstApp
dotnet run</code></pre>
</div>'

DECLARE @ReadContent2 NVARCHAR(MAX) = N'<div class="lesson-content">
    <h1>Giới thiệu REST APIs</h1>
    <p class="lead">REST là kiến trúc phổ biến để xây dựng web services.</p>
    <h2>HTTP Methods</h2>
    <table class="data-table">
        <tr><td><code>GET</code></td><td>Lấy dữ liệu</td></tr>
        <tr><td><code>POST</code></td><td>Tạo resource mới</td></tr>
        <tr><td><code>PUT</code></td><td>Cập nhật resource</td></tr>
        <tr><td><code>DELETE</code></td><td>Xóa resource</td></tr>
    </table>
</div>'

INSERT INTO [Lessons] ([lesson_id], [module_id], [title], [type], [content_url], [video_url], [content], [duration], [order_index], [created_at]) VALUES
(NEWID(), @Module1_1, 'Chào mừng đến với ASP.NET Core', 'Read', NULL, NULL, @ReadContent1, 10, 1, GETUTCDATE()),
(NEWID(), @Module1_1, 'Cài đặt .NET SDK', 'Video', @VideoUrl1, @VideoUrl1, NULL, 15, 2, GETUTCDATE()),
(NEWID(), @Module1_2, 'Hiểu về MVC Pattern', 'Video', @VideoUrl2, @VideoUrl2, NULL, 20, 1, GETUTCDATE()),
(NEWID(), @Module1_2, 'Tạo Controllers', 'Video', @VideoUrl1, @VideoUrl1, NULL, 25, 2, GETUTCDATE()),
(NEWID(), @Module2_1, 'Node.js là gì?', 'Video', @VideoUrl2, @VideoUrl2, NULL, 15, 1, GETUTCDATE()),
(NEWID(), @Module2_1, 'npm và Package Management', 'Video', @VideoUrl1, @VideoUrl1, NULL, 20, 2, GETUTCDATE()),
(NEWID(), @Module2_2, 'Giới thiệu REST APIs', 'Read', NULL, NULL, @ReadContent2, 20, 1, GETUTCDATE()),
(NEWID(), @Module2_2, 'Xây dựng API đầu tiên', 'Video', @VideoUrl2, @VideoUrl2, NULL, 30, 2, GETUTCDATE()),
(NEWID(), @Module3_1, 'Bắt đầu với Python', 'Video', @VideoUrl1, @VideoUrl1, NULL, 15, 1, GETUTCDATE()),
(NEWID(), @Module3_1, 'Các kiểu dữ liệu Python', 'Video', @VideoUrl2, @VideoUrl2, NULL, 20, 2, GETUTCDATE()),
(NEWID(), @Module3_2, 'Giới thiệu Pandas', 'Video', @VideoUrl1, @VideoUrl1, NULL, 25, 1, GETUTCDATE()),
(NEWID(), @Module3_2, 'Trực quan hóa dữ liệu', 'Video', @VideoUrl2, @VideoUrl2, NULL, 30, 2, GETUTCDATE()),
(NEWID(), @Module4_1, 'Giới thiệu SQL', 'Video', @VideoUrl2, @VideoUrl2, NULL, 15, 1, GETUTCDATE()),
(NEWID(), @Module4_1, 'Câu lệnh SELECT', 'Video', @VideoUrl1, @VideoUrl1, NULL, 25, 2, GETUTCDATE()),
(NEWID(), @Module4_2, 'JOIN Operations', 'Video', @VideoUrl2, @VideoUrl2, NULL, 30, 1, GETUTCDATE()),
(NEWID(), @Module4_2, 'Subqueries và CTEs', 'Video', @VideoUrl1, @VideoUrl1, NULL, 25, 2, GETUTCDATE()),
(NEWID(), @Module5_1, 'HTML5 Cơ bản', 'Video', @VideoUrl1, @VideoUrl1, NULL, 20, 1, GETUTCDATE()),
(NEWID(), @Module5_1, 'Semantic HTML Elements', 'Video', @VideoUrl2, @VideoUrl2, NULL, 25, 2, GETUTCDATE()),
(NEWID(), @Module5_2, 'CSS Selectors', 'Video', @VideoUrl1, @VideoUrl1, NULL, 25, 1, GETUTCDATE()),
(NEWID(), @Module5_2, 'Flexbox Layout', 'Video', @VideoUrl2, @VideoUrl2, NULL, 30, 2, GETUTCDATE());

-- =============================================
-- 8. COURSE REVIEWS
-- =============================================
INSERT INTO [Course_Reviews] ([review_id], [course_id], [user_id], [rating], [comment], [created_at]) VALUES
(NEWID(), @Course1Id, @Student1Id, 5, 'Khóa học tuyệt vời! Giảng viên giải thích rất dễ hiểu.', DATEADD(DAY, -5, GETUTCDATE())),
(NEWID(), @Course1Id, @Student3Id, 4, 'Nội dung đầy đủ, rất phù hợp cho người mới bắt đầu.', DATEADD(DAY, -3, GETUTCDATE())),
(NEWID(), @Course2Id, @Student1Id, 5, 'Khóa Node.js hay nhất mà tôi đã học. REST API section rất chi tiết.', DATEADD(DAY, -7, GETUTCDATE())),
(NEWID(), @Course2Id, @Student2Id, 4, 'Khóa học tốt, nên cập nhật thêm phiên bản mới nhất.', DATEADD(DAY, -2, GETUTCDATE())),
(NEWID(), @Course3Id, @Student3Id, 5, 'Jane là một giảng viên tuyệt vời! Phần machine learning rất ấn tượng.', DATEADD(DAY, -4, GETUTCDATE())),
(NEWID(), @Course4Id, @Student1Id, 5, 'Cuối cùng đã hiểu SQL đúng cách! Bài tập thực hành rất hữu ích.', DATEADD(DAY, -6, GETUTCDATE())),
(NEWID(), @Course5Id, @Student2Id, 4, 'Rất tốt cho người mới. Nên thêm phần CSS animations.', DATEADD(DAY, -1, GETUTCDATE())),
(NEWID(), @Course6Id, @Student1Id, 5, 'Web API được giải thích rõ ràng. Authentication rất hay.', DATEADD(DAY, -8, GETUTCDATE())),
(NEWID(), @Course12Id, @Student2Id, 5, 'React hooks và Redux được dạy rất chi tiết. Highly recommended!', DATEADD(DAY, -9, GETUTCDATE())),
(NEWID(), @Course18Id, @Student3Id, 5, 'Docker từ cơ bản đến nâng cao. Project thực tế rất hữu ích.', DATEADD(DAY, -10, GETUTCDATE()));

-- =============================================
-- 9. ENROLLMENTS
-- =============================================
INSERT INTO [Enrollments] ([enrollment_id], [user_id], [course_id], [enrolled_at], [status]) VALUES
(NEWID(), @Student1Id, @Course1Id, DATEADD(DAY, -10, GETUTCDATE()), 'Active'),
(NEWID(), @Student1Id, @Course2Id, DATEADD(DAY, -8, GETUTCDATE()), 'Active'),
(NEWID(), @Student1Id, @Course4Id, DATEADD(DAY, -12, GETUTCDATE()), 'Completed'),
(NEWID(), @Student1Id, @Course6Id, DATEADD(DAY, -15, GETUTCDATE()), 'Active'),
(NEWID(), @Student2Id, @Course2Id, DATEADD(DAY, -5, GETUTCDATE()), 'Active'),
(NEWID(), @Student2Id, @Course5Id, DATEADD(DAY, -3, GETUTCDATE()), 'Active'),
(NEWID(), @Student2Id, @Course12Id, DATEADD(DAY, -7, GETUTCDATE()), 'Active'),
(NEWID(), @Student3Id, @Course1Id, DATEADD(DAY, -15, GETUTCDATE()), 'Active'),
(NEWID(), @Student3Id, @Course3Id, DATEADD(DAY, -7, GETUTCDATE()), 'Active'),
(NEWID(), @Student3Id, @Course18Id, DATEADD(DAY, -9, GETUTCDATE()), 'Active');

-- =============================================
-- 10. LEARNING PATHS
-- =============================================
DECLARE @Path1Id UNIQUEIDENTIFIER = NEWID()
DECLARE @Path2Id UNIQUEIDENTIFIER = NEWID()
DECLARE @Path3Id UNIQUEIDENTIFIER = NEWID()

INSERT INTO [Learning_Paths] ([path_id], [title], [description], [price], [status], [is_custom_path], [created_at], [created_by_user_id]) VALUES
(@Path1Id, 'Full-Stack .NET Developer', 'Trở thành lập trình viên full-stack với ASP.NET Core! Học frontend, backend, và database.', 1499000, 'Published', 0, GETUTCDATE(), @Instructor1Id),
(@Path2Id, 'Backend Developer Career Path', 'Làm chủ backend development với .NET, Node.js, và SQL Server.', 1299000, 'Published', 0, GETUTCDATE(), @Instructor1Id),
(@Path3Id, 'Data Science với Python', 'Phân tích dữ liệu và machine learning với Python từ cơ bản đến nâng cao.', 1199000, 'Published', 0, GETUTCDATE(), @Instructor2Id);

INSERT INTO [Path_Courses] ([path_id], [course_id], [order_index]) VALUES
(@Path1Id, @Course5Id, 1),
(@Path1Id, @Course1Id, 2),
(@Path1Id, @Course4Id, 3),
(@Path1Id, @Course6Id, 4),
(@Path2Id, @Course1Id, 1),
(@Path2Id, @Course2Id, 2),
(@Path2Id, @Course4Id, 3),
(@Path3Id, @Course3Id, 1),
(@Path3Id, @Course9Id, 2);

PRINT 'Seed data inserted successfully! (20 courses with VND prices)'
PRINT 'Test Accounts:'
PRINT '  Admin: admin@learnhub.com / Password123!'
PRINT '  Instructor: john.smith@learnhub.com / Password123!'
PRINT '  Student: alice.nguyen@gmail.com / Password123!'
